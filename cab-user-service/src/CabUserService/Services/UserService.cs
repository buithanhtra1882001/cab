using AutoMapper;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CabNotificationService.Constants;
using CabUserService.Constants;
using CabUserService.Cqrs.Requests.Commands;
using CabUserService.Cqrs.Requests.Queries;
using CabUserService.Infrastructures.Communications.Http;
using CabUserService.Infrastructures.DbContexts;
using CabUserService.Infrastructures.Exceptions;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.IntegrationEvents.Events;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;
using CabUserService.Services.Base;
using CabUserService.Services.Interfaces;
using LazyCache;
using MassTransit;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace CabUserService.Services
{
    public class UserService : BaseService<UserService>, IUserService
    {
        private IServiceProvider _serviceProvider;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IHttpClientWrapper _httpClientWrapper;
        private readonly string _baseUrl;
        private readonly IAppCache _cache;
        private readonly PostgresDbContext _postgresDbContext;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        private static readonly int LeaderboardCount = 10;

        public UserService(
            ILogger<UserService> logger,
            IServiceProvider serviceProvider,
            IEmailService emailService,
            IConfiguration configuration,
            IAppCache cache,
            IMapper mapper,
            IHttpClientWrapper httpClientWrapper,
            PostgresDbContext postgresDbContext,
            IHttpContextAccessor httpContextAccessor)
        : base(logger)
        {
            _serviceProvider = serviceProvider;
            _emailService = emailService;
            _configuration = configuration;
            _cache = cache;
            _mapper = mapper;
            _httpClientWrapper = httpClientWrapper;
            _baseUrl = configuration.GetValue<string>("GlobalService:BaseAddress");
            _postgresDbContext = postgresDbContext;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<string> ConfirmCreatorAsync(Guid userId)
        {
            try
            {
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var userEntity = await userRepository.GetByIdAsync(userId);
                if (userEntity is null)
                {
                    return $"Not found user with Id ={userId}";
                }
                if (userEntity.UserType == UserType.CONTENT_CREATOR)
                {
                    return $"User is already a content creator";
                }
                userEntity.UserType = UserType.CONTENT_CREATOR;
                await userRepository.UpdateTypeAsync(userEntity, true);
                return "Confirmed successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> RequestCreatorAsync(Guid userId)
        {
            try
            {
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var userEntity = await userRepository.GetByIdAsync(userId);
                if (userEntity is null)
                {
                    return $"Not found user with Id ={userId}";
                }
                if (userEntity.IsRequestCreator == true)
                {
                    return $"Request has already been sent";
                }
                userEntity.IsRequestCreator = true;
                await userRepository.UpdateTypeAsync(userEntity, false);
                return "Request has been sent successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task NotifyUserCreatorsAsync()
        {
            try
            {
                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
                var eventBus = _serviceProvider.GetRequiredService<IEventBus>();
                var usersWithFollower = await db.Users.Include(x => x.UserDetail).Select(x => new
                {
                    x.Id,
                    x.UserName,
                    x.Email,
                    x.UserDetail.Follower,
                    x.UserType
                }).ToListAsync();
                var usersToProcess = usersWithFollower
                .Where(x => x.Follower.Split(',').Length >= 5000 && x.UserType == UserType.NORMAL)
                .ToList();

                var actorInfo = await GetSystemActorInForAsync();


                var emailTasks = usersToProcess.Select(user =>
                {
                    eventBus.Publish(new NotificationIntegrationEvent
                        (
                         userIds: new List<Guid> { user.Id },
                         actor: actorInfo,
                         referenceId: user.Id,
                         notificationType: NotificationConstant.QualifiedCreator,
                         donateAmount: null,
                         referenceUrl: $"{_configuration["ClientService:BaseAddress"]}/settings",
                         type: NotificationConstant.System
                         ));

                    var emailConfig = new SendEmailConfig(
                           "CAB",
                           "cabplatformsvn@gmail.com",
                           user.UserName,
                           user.Email,
                           "Tài khoản của bạn đủ điều kiện trở thành User Creator",
                           $"<p>Hãy click vào <a href=\"{_baseUrl}/users/request-creator?userId={user.Id}\">đây</a> để xác thực.</p><p>Nếu <strong>không phải là bạn thực hiện điều này</strong>" +
                           $", tuyệt đối không click vào đường dẫn hoặc cung cấp đường dẫn cho bất cứ ai.</p><p>Trân trọng.</p><strong>CAB.VN</strong>");

                    return _emailService.SendAsync(emailConfig);
                });
                await Task.WhenAll(emailTasks);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during background task execution.");
                throw;
            }
        }

        /// <summary>
        /// Lấy dữ liệu đề xuất bạn bè
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<UserRequestFriendDto>> GetRequestFriendAsync(Guid userId)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var cache = _serviceProvider.GetRequiredService<IAppCache>();
            var userService = _serviceProvider.GetRequiredService<IUserRepository>();
            var userImageRepository = _serviceProvider.GetRequiredService<IUserImageRepository>();

            var result = await userService.GetRequestFriend(userId);
            if (result == null || !result.Any())
            {
                var userRequestFriendActions = await db.UserRequestFriendActions
                .Where(y => y.UserId == userId || y.RequestUserId == userId)
                .ToListAsync();

                var currentIds = userRequestFriendActions.Select(x => x.UserId).ToList();
                var requestUserIds = userRequestFriendActions.Select(x => x.RequestUserId).ToList();
                var allUserIds = currentIds.Union(requestUserIds).ToList();

                var userEntities = await db.Users
                    .Where(x => x.Id != userId && !allUserIds.Contains(x.Id))
                    .Take(5).Select(x => new { x.Id, x.UserName })
                    .ToListAsync();
                result = userEntities.Select(x => new UserRequestFriendDto
                {
                    RequestUserId = x.Id,
                    RequestFullName = x.UserName
                }).ToList();
            }

            if (result != null && result.Any())
            {
                var currentUser = await db.Users
                    .Where(x => x.Id == userId)
                    .Include(x => x.UserDetail)
                    .FirstOrDefaultAsync();
                var userIds = result.Select(user => user.RequestUserId).ToList();
                var userImages = await userImageRepository.GetListByUserIdAsync(userIds);

                foreach (var user in result)
                {
                    var userImage = userImages.FirstOrDefault(img => img.UserId == user.RequestUserId);
                    user.Avatar = userImage?.Url ?? string.Empty;
                    user.IsFollow = string.Join(";", currentUser.UserDetail.Follower).Contains(user.RequestUserId.ToString());
                }
            }

            return result ?? new List<UserRequestFriendDto>();
            //Func<Task<List<UserRequestFriendDto>>> objectFactory = () => userService.GetRequestFriend(userId);

            //var result = (await cache.GetOrAddAsync(
            //        $"{CacheKeyConstant.USER_REQUEST_FRIEND}_{userId}",
            //        objectFactory,
            //        DateTimeOffset.Now.AddMinutes(5))).ToList();
        }

        //UserRequestFriendAction
        public async Task<string> AddFriendRequestAsync(Guid auid, RequestFriendRequest request)
        {
            try
            {
                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
                var profileService = _serviceProvider.GetRequiredService<IProfileService>();
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var userImageRepository = _serviceProvider.GetRequiredService<IUserImageRepository>();
                var currentUser = await userRepository.GetByIdAsync(auid);
                var requestUser = await userRepository.GetByIdAsync(request.UserId);
                if (currentUser is null || requestUser is null)
                {
                    return $"Not found user with Id ={auid} or {request.UserId}";
                }

                if (await db.UserRequestFriendActions.AnyAsync(x => x.UserId == request.UserId && x.RequestUserId == auid))
                {
                    return "User friend request already exists";
                }
                if (request.TypeRequest == REQUEST_TYPE.FRIEND)
                {
                    var newFriendRequest = _mapper.Map<UserRequestFriendAction>(request);
                    newFriendRequest.Id = Guid.NewGuid();
                    newFriendRequest.RequestUserId = auid;
                    newFriendRequest.AcceptStatus = ACCEPTANCE_STATUS.NORMAL;
                    db.UserRequestFriendActions.Add(newFriendRequest);
                    await db.SaveChangesAsync();

                    var eventBus = _serviceProvider.GetRequiredService<IEventBus>();

                    var userImg = await userImageRepository.GetByIdAsync(currentUser.Id);
                    var actorInfo = new UserInfo
                        (
                        userId: currentUser.Id,
                        fullName: currentUser.FullName,
                        avatar: userImg?.Url
                        );

                    eventBus.Publish(new NotificationIntegrationEvent
                       (
                        userIds: new List<Guid> { requestUser.Id },
                        actor: actorInfo,
                        referenceId: currentUser.Id,
                        notificationType: NotificationConstant.FriendRequest,
                        donateAmount: null,
                        referenceUrl: null,
                        type: null
                        ));
                }
                else if (request.StatusAction == ACTION_FRIEND_TYPE.SEND_REQUEST)
                {
                    await profileService.AddFollowerAsync(request.UserId, auid.ToString());
                }
                else
                {
                    await UpdateIsFollowerAsync(request.UserId, true);
                }

                return "Request success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AddFriendRequestAsync");
                throw;
            }
        }

        public async Task<string> AddFriendAsync(Guid auid, AcceptFriendRequest request)
        {
            try
            {
                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
                var userImageRepository = _serviceProvider.GetRequiredService<IUserImageRepository>();
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var currentUser = await userRepository.GetByIdAsync(auid);
                var requestUser = await userRepository.GetByIdAsync(request.RequestUserId);
                if (currentUser is null || requestUser is null)
                {
                    return $"Not found user with Id ={auid} or {request.UserId}";
                }


                if (await db.UserFriends.AnyAsync(x => (x.UserId == auid && x.FriendId == request.RequestUserId)
                || (x.UserId == request.RequestUserId && x.FriendId == auid)))
                {
                    return "User friend already exists";
                }

                var friendRequest = await db.UserRequestFriendActions
                         .FirstOrDefaultAsync(x => (x.UserId == auid && x.RequestUserId == request.RequestUserId)
                         || (x.UserId == request.RequestUserId && x.RequestUserId == auid));
                if (friendRequest is null)
                {
                    return "User request friend not found";
                }

                if (request.AcceptStatus == ACCEPTANCE_STATUS.ACCEPTED)
                {
                    var userFriend = new UserFriend
                    {
                        UserId = auid,
                        FriendId = request.RequestUserId
                    };
                    await db.UserFriends.AddAsync(userFriend);
                }

                friendRequest.AcceptStatus = request.AcceptStatus;
                db.UserRequestFriendActions.Update(friendRequest);
                await db.SaveChangesAsync();

                var eventBus = _serviceProvider.GetRequiredService<IEventBus>();

                var userImg = await userImageRepository.GetByIdAsync(currentUser.Id);
                var actorInfo = new UserInfo
                    (
                    userId: currentUser.Id,
                    fullName: currentUser.FullName,
                    avatar: userImg?.Url
                    );

                eventBus.Publish(new NotificationIntegrationEvent
                   (
                    userIds: new List<Guid> { requestUser.Id },
                    actor: actorInfo,
                    referenceId: currentUser.Id,
                    notificationType: NotificationConstant.FriendRequest,
                    donateAmount: null,
                    referenceUrl: null,
                    type: null
                    ));

                return "Add friend success";

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in AddFriendRequestAsync");
                throw;
            }
        }

        public async Task<List<RequestFriendResponse>> GetFriendRequestAsync(Guid auid)
        {
            try
            {
                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var userEntity = await userRepository.GetByIdAsync(auid);
                var userImageRepository = _serviceProvider.GetRequiredService<IUserImageRepository>();
                if (userEntity is null)
                {
                    _logger.LogError($"Not found user with Id={auid}");
                    throw new InvalidOperationException($"Not found user with Id={auid}");
                }
                var users = await db.UserRequestFriendActions
                                    .Where(userRequest => userRequest.UserId == userEntity.Id
                                     && userRequest.StatusAction == ACTION_FRIEND_TYPE.SEND_REQUEST && userRequest.AcceptStatus == ACCEPTANCE_STATUS.NORMAL)
                                    .Join(db.Users, userRequest => userRequest.RequestUserId, user => user.Id, (userRequest, user)
                => new { user.Id, user.FullName }).ToListAsync();

                if (users == null || !users.Any())
                {
                    return new List<RequestFriendResponse>();
                }

                var userIds = users.Select(user => user.Id).ToList();
                var userImages = await userImageRepository.GetListByUserIdAsync(userIds);

                var requestFriendResponse = users.Select(user =>
                {
                    var userImage = userImages?.FirstOrDefault(x => x.UserId == user.Id);
                    return new RequestFriendResponse
                    {
                        UserId = user.Id,
                        Avatar = userImage?.Url,
                        FullName = user.FullName
                    };
                }).ToList();

                return requestFriendResponse;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetFriendRequestAsync");
                throw;
            }
        }

        public async Task<List<CreatorResponse>> GetRequestCreatorAsync(string bearerToken, Guid userId)
        {
            try
            {
                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

                var userSimilarityRequest = await GetUserSimilarityRequestAsnyc(db, userId);
                if (userSimilarityRequest is null)
                {
                    var errorMessage = $"Not found user with Id ={userId}";
                    _logger.LogError(errorMessage);
                    throw new Exception(errorMessage);
                }

                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

                var creatorIds = await userRepository.GetCreatorsAsync(userSimilarityRequest);
                var result = await GetCreatorsFromPostServiceAsync(bearerToken, creatorIds, userId);

                if (result == null || !result.Any())
                {
                    result = await GetDefaultCreatorsAsync(userId) ?? new List<CreatorResponse>();
                }

                return result;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GetRequestCreatorAsync");
                throw;
            }

        }

        public async Task<PagingResponse<UserFriendResponse>> GetUserFriendsAsync(GetUserFriendsRequest request)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var userImageRepository = _serviceProvider.GetRequiredService<IUserImageRepository>();

            var queryUserFriends = db.UserFriends
               .AsNoTracking()
               .Where(x => x.UserId == request.UserId || x.FriendId == request.UserId);

            var count = await queryUserFriends.CountAsync();

            var items = new List<UserFriend>();

            if (request.PageNumber > 0 && request.PageSize > 0)
            {
                items = await queryUserFriends
                  .Skip((request.PageNumber - 1) * request.PageSize)
                  .Take(request.PageSize)
                  .ToListAsync();
            }
            else
            {
                items = await queryUserFriends.ToListAsync();
            }

            var lstUserFriendId = items.SelectMany(x => new[] { x.UserId, x.FriendId })
                .Distinct().ToList();
            lstUserFriendId.Remove(request.UserId);

            var users = await db.Users.Include(x => x.UserDetail)
                                      .Where(x => lstUserFriendId.Contains(x.Id))
                                      .Select(x => new UserFriendResponse
                                      {
                                          FullName = x.FullName,
                                          UserId = x.Id
                                      })
                                      .ToListAsync();

            var userIds = users.Select(x => x.UserId).ToList();
            var userImages = await userImageRepository.GetListByUserIdAsync(userIds);

            foreach (var user in users)
            {
                var userImage = userImages.FirstOrDefault(img => img.UserId == user.UserId);
                user.Avatar = userImage?.Url ?? string.Empty;
            }

            return new PagingResponse<UserFriendResponse>
            {
                Data = users,
                Total = count
            };
        }

        public async Task<string> UnfriendAsync(Guid auid, Guid friendId)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

            var userEntity = await userRepository.GetByIdAsync(auid);
            var friendEntity = await userRepository.GetByIdAsync(friendId);

            if (userEntity is null)
            {
                return $"User with Id = {auid} not found";
            }

            if (friendEntity is null)
            {
                return $"User with Id = {friendId} not found";
            }

            using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    var userFriendEntity = await db.UserFriends
                        .FirstOrDefaultAsync(x => (x.UserId == auid && x.FriendId == friendId)
                        || (x.UserId == friendId && x.FriendId == auid));

                    var userRequestFriendEntity = await db.UserRequestFriendActions
                        .FirstOrDefaultAsync(x => (x.UserId == auid && x.RequestUserId == friendId)
                    || (x.UserId == friendId && x.RequestUserId == auid));

                    if (userFriendEntity is null && userRequestFriendEntity is null)
                    {
                        return "Friendship does not exist";
                    }

                    if (userFriendEntity != null)
                    {

                        db.UserFriends.Remove(userFriendEntity);
                    }

                    if (userRequestFriendEntity != null)
                    {
                        db.UserRequestFriendActions.Remove(userRequestFriendEntity);
                    }

                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return userFriendEntity != null ? "Unfriend successful" : "Successfully canceled friend request";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new AppException($"An error occurred while unfriending user with Id = {auid} and friend with Id = {friendId}");
                }
            }
        }

        #region CHAT
        public async Task<PagingResponse<UserMessageResponse>> GetMessagesByUserIdAsync(GetUserMessagesByUserIdRequest request)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var chatMessageUserIdMaterializedViewRepository = _serviceProvider.GetRequiredService<IChatMessageUserIdMaterializedViewRepository>();
            var userImageRepository = _serviceProvider.GetRequiredService<IUserImageRepository>();

            var userEntity = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (userEntity is null)
            {
                var errorMessage = $"Not found user with Id ={request.UserId}";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            var userMessages = await chatMessageUserIdMaterializedViewRepository
                .GetListByUserIdAsync(userEntity.Id);

            userMessages = userMessages
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var allUserIds = userMessages.SelectMany(x => new[] { x.SenderUserId, x.RecipientUserId })
                .Distinct()
                .ToList();

            allUserIds.Remove(userEntity.Id);

            var userMessageResponse = await db.Users.Include(x => x.UserDetail)
                                                    .Where(x => allUserIds.Contains(x.Id))
                                                    .Select(x => new UserMessageResponse
                                                    {
                                                        UserId = x.Id,
                                                        Avatar = x.UserDetail.Avatar,
                                                        FullName = x.FullName,
                                                    })
                                                    .ToListAsync();

            var userIds = userMessageResponse.Select(x => x.UserId).ToList();
            var userImages = await userImageRepository.GetListByUserIdAsync(userIds);
            foreach (var user in userMessageResponse)
            {
                var userImage = userImages.FirstOrDefault(img => img.UserId == user.UserId);
                user.Avatar = userImage?.Url ?? string.Empty;
            }

            var response = new PagingResponse<UserMessageResponse>()
            {
                Total = userMessageResponse.Count(),
                Data = userMessageResponse,
                HasNext = userMessageResponse.Count() == request.PageSize,
            };
            return response;
        }

        public async Task<PagingResponse<UserOnlineResponse>> GetUserFriendOnlineAsync(UserFriendOnlineRequest request)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var userImageRepository = _serviceProvider.GetRequiredService<IUserImageRepository>();

            var userEntity = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
            var response = new PagingResponse<UserOnlineResponse>();
            if (userEntity is null)
            {
                var errorMessage = $"Not found user with Id ={request.UserId}";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            var friendEntities = await db.UserFriends
                .Where(x => x.UserId == userEntity.Id || x.FriendId == userEntity.Id)
                .ToListAsync();

            var friendIds = friendEntities.SelectMany(x => new[] { x.UserId, x.FriendId }).Distinct().ToList();
            friendIds.Remove(userEntity.Id);

            if (!friendIds.Any())
            {
                return response;
            }

            var userOnlineResponse = await db.ChatUserConnections
                .Where(x => friendIds.Contains(x.UserId))
                .Join(db.Users, uc => uc.UserId, u => u.Id, (uc, u) => new { uc, u })
                .SelectMany(ue => db.UserDetails.Where(ud => ud.UserId == ue.u.Id).DefaultIfEmpty()
                , (ue, ud) => new UserOnlineResponse
                {
                    UserId = ue.u.Id,
                    Avatar = ud.Avatar,
                    FullName = ue.u.FullName,
                    IsOnline = ue.uc.IsOnline,
                    LastTime = ue.uc.LastTime
                })
                .GroupBy(x => x.UserId)
                .Select(g => g.OrderByDescending(x => x.LastTime).First())
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var userIds = userOnlineResponse.Select(x => x.UserId).ToList();
            var userImages = await userImageRepository.GetListByUserIdAsync(userIds);
            foreach (var user in userOnlineResponse)
            {
                var userImage = userImages.FirstOrDefault(img => img.UserId == user.UserId);
                user.Avatar = userImage?.Url ?? string.Empty;
            }

            response.Total = await db.ChatUserConnections.CountAsync(x => friendIds.Contains(x.UserId));
            response.Data = userOnlineResponse;
            response.HasNext = userOnlineResponse.Count() == request.PageSize;

            return response;
        }
        public async Task<bool> IsUserOnlineAsync(Guid id)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            var existUserConnection = await db.ChatUserConnections
                .Where(x => x.UserId == id)
                .OrderByDescending(x => x.LastTime).FirstOrDefaultAsync();

            return existUserConnection.IsOnline;
        }
        public async Task<PagingStateResponse<MessagesResponse>> GetContentMessagesAsync(GetContentMessageRequest request)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var chatMessageUserIdMaterializedViewRepository = _serviceProvider.GetRequiredService<IChatMessageUserIdMaterializedViewRepository>();

            var currentUserEntity = await db.Users
                            .FirstOrDefaultAsync(x => x.Id == request.CurrentUserId);
            var friendEntity = await db.Users
                             .FirstOrDefaultAsync(x => x.Id == request.FriendUserId);
            if (currentUserEntity is null || friendEntity is null)
            {
                var errorMessage = $"Not found User with userId = {request.CurrentUserId} || userFriendId = {request.FriendUserId}";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            byte[] pagingStateFirstByte = string.IsNullOrEmpty(request.PagingStateFirst) ? null : Convert.FromBase64String(request.PagingStateFirst);
            byte[] pagingStateLastByte = string.IsNullOrEmpty(request.PagingStateLast) ? null : Convert.FromBase64String(request.PagingStateLast);

            var (data, (pagingStateFirst, pagingStateLast)) = await chatMessageUserIdMaterializedViewRepository
                .GetListByConditionOrPagingAsync
                (x => x.SenderUserId == request.CurrentUserId && x.RecipientUserId == request.FriendUserId,
                 x => x.SenderUserId == request.FriendUserId && x.RecipientUserId == request.CurrentUserId,
                 request.PageSize,
                 pagingStateFirstByte,
                 pagingStateLastByte);

            var messagesResponse = _mapper.Map<List<MessagesResponse>>(data);

            if (messagesResponse.Any())
            {
                var senderAndRecipientIds = messagesResponse.Distinct()
                .SelectMany(x => new[] { x.SenderUserId, x.RecipientUserId })
                .ToList();

                var listUser = await db.Users
                    .Where(x => senderAndRecipientIds
                    .Contains(x.Id)).ToListAsync();

                foreach (var message in messagesResponse)
                {
                    var sender = listUser.FirstOrDefault(x => x.Id == message.SenderUserId);
                    var recipient = listUser.FirstOrDefault(x => x.Id == message.RecipientUserId);
                    message.SenderName = sender.FullName;
                    message.RecipientName = recipient.FullName;
                }
            }

            var response = new PagingStateResponse<MessagesResponse>()
            {
                Total = messagesResponse.Count(),
                Data = messagesResponse,
                PagingStateFirst = pagingStateFirst,
                PagingStateLast = pagingStateLast,
                HasNext = !(pagingStateFirst == "" && pagingStateLast == "" && messagesResponse.Count() <= request.PageSize),
            };
            return response;
        }

        public async Task<LeaderBoardResponse> GetLeaderBoardAsync()
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            var userTransactionLogs = await db.UserTransactionLogs.AsNoTracking().ToListAsync();
            var donationTransactions = userTransactionLogs.Where(x => x.Type == TransactionType.TRANSFER && x.Status == TransactionStatus.SUCCESS);
            var topDonators = GetTopDonators(donationTransactions, db);
            var topReceivers = GetTopReceivers(donationTransactions, db);
            var topIsFollowed = await GetTopUsersFollowAsync(db);

            var response = new LeaderBoardResponse()
            {
                UserFollows = topIsFollowed,
                UserDonates = topDonators,
                UserRecieveDonates = topReceivers
            };

            return response;
        }

        public async Task<bool> UpdateProfileAsync(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            bool result = false;
            var isExistUser = await _postgresDbContext.Users.AsQueryable()
                .Where(x => x.Id == request.UserId)
                .Include(x => x.UserDetail)
                .FirstOrDefaultAsync(cancellationToken);
            if (isExistUser == null)
                throw new Exception($"Not found user with Id ={request.UserId}");

            var lstFllowCategories = await _postgresDbContext.Categories.AsQueryable().
                Where(x => request.CategoryFavorites.Contains(x.Id))
                .Select(y => new UserCategory()
                {
                    UserId = request.UserId,
                    CategoryId = y.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }).ToListAsync(cancellationToken);

            using (var transaction = _postgresDbContext.Database.BeginTransaction())
            {

                try
                {
                    isExistUser.UserDetail.Marry = request.Marry;
                    isExistUser.UserDetail.SexualOrientation = request.SexualOrientation;
                    isExistUser.UserDetail.School = string.Join(",", request.Schools);
                    isExistUser.UserDetail.Company = string.Join(",", request.Companys);
                    isExistUser.UserDetail.HomeLand = request.HomeLand;
                    isExistUser.UserDetail.IsShowSexualOrientation = request.IsShowSexualOrientation;
                    isExistUser.UserDetail.IsShowMarry = request.IsShowMarry;
                    isExistUser.UserDetail.IsShowSchool = request.IsShowSchool;
                    isExistUser.UserDetail.IsShowCompany = request.IsShowCompany;
                    isExistUser.UserDetail.IsShowHomeLand = request.IsShowHomeLand;

                    isExistUser.UserDetail.IsShowDob = request.IsShowDob;
                    isExistUser.UserDetail.IsShowEmail = request.IsShowEmail;
                    isExistUser.UserDetail.IsShowPhone = request.IsShowPhone;
                    isExistUser.UserDetail.IsShowCity = request.IsShowCity;
                    isExistUser.UserDetail.IsShowDescription = request.IsShowDescription;

                    isExistUser.UserDetail.UpdatedAt = DateTime.Now;
                    _postgresDbContext.UserDetails.Update(isExistUser.UserDetail);
                    await _postgresDbContext.SaveChangesAsync();

                    _postgresDbContext.UserCategories.RemoveRange(lstFllowCategories);
                    await _postgresDbContext.UserCategories.AddRangeAsync(lstFllowCategories, cancellationToken);
                    await _postgresDbContext.SaveChangesAsync();

                    transaction.Commit();
                    result = true;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new Exception($"Error internal server. Please try again.");
                }
            }

            return result;
        }

        public async Task<StatisticalUserDto> GetStatisticalUserAsync(StatisticalUserQuery request, CancellationToken cancellationToken)
        {
            var isUserExist = await _postgresDbContext.Users.AsQueryable().AsNoTracking()
                .Where(x => x.Id == request.UserId)
                .Include(dt => dt.UserDetail)
                .FirstOrDefaultAsync(cancellationToken);
            if (isUserExist == null)
                throw new Exception($"Not found user with Id ={request.UserId}");

            // get new friend
            var queryNewfriend = _postgresDbContext.UserFriends.AsNoTracking()
            .Where(x => x.UserId == request.UserId || x.FriendId == request.UserId)
                .AsQueryable();
            if (request.FromDate != null)
            {
                queryNewfriend = queryNewfriend.Where(x => request.FromDate >= x.CreatedAt).AsQueryable();
            }

            if (request.ToDate != null)
            {
                queryNewfriend = queryNewfriend.Where(x => request.ToDate <= x.CreatedAt).AsQueryable();
            }

            var topDonate = await _postgresDbContext.Users.AsQueryable().AsNoTracking()
                .Where(x => x.Id.ToString() == request.DonaterId)
                .FirstOrDefaultAsync(cancellationToken);

            var topReac = await _postgresDbContext.Users.AsQueryable().AsNoTracking()
                .Where(x => x.Id.ToString() == request.ReactionId)
                .FirstOrDefaultAsync(cancellationToken);

            var result = await queryNewfriend.ToListAsync(cancellationToken);

            return new StatisticalUserDto()
            {
                NewFriend = result.Count(),
                IsUpdateProfile = isUserExist.UserDetail.IsUpdateProfile,
                topDonate = new TopDonate()
                {
                    name = topDonate != null ? topDonate.UserName : string.Empty
                },
                topReaction = new TopReaction()
                {
                    name = topReac != null ? topReac.UserName : string.Empty
                }
            };
        }

        #endregion

        #region PRIVATE
        private async Task<List<StatsticUserFollow>> GetTopUsersFollowAsync(PostgresDbContext dbContext)
        {
            var userImageRepository = _serviceProvider.GetRequiredService<IUserImageRepository>();
            var userFollows = await dbContext.UserFollowHistories
                .GroupBy(u => u.UserId)
                .Select(g => new { UserId = g.Key, TotalFollow = g.Count() })
                .OrderByDescending(x => x.TotalFollow)
                .Take(LeaderboardCount)
                .ToListAsync();

            if (!userFollows.Any())
                return new List<StatsticUserFollow>();

            var userIds = userFollows.Select(x => x.UserId).ToList();
            var userFollowImages = await userImageRepository.GetListByUserIdAsync(userIds);

            var userDetails = await dbContext.UserDetails
                .Where(x => userIds.Contains(x.UserId))
                .Include(x => x.User)
                .ToListAsync();

            return userDetails.Select(x => new StatsticUserFollow
            {
                UserId = x.UserId,
                Avatar = x.Avatar,
                UserName = x.User == null ? "" : x.User.FullName,
                TotalFollow = userFollows.FirstOrDefault(y => y.UserId == x.UserId).TotalFollow
            }).OrderByDescending(x => x.TotalFollow).ToList();
        }

        private async Task<List<CreatorResponse>> GetCreatorsFromPostServiceAsync(string bearerToken, List<Guid> creatorIds, Guid userId)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            var userFollower = await db.UserDetails.Where(x => creatorIds.Contains(x.UserId))
                                                       .Select(x => new { x.Follower, x.UserId })
                                                       .ToListAsync();

            var filterCreatorIds = userFollower
                .Where(x => string.IsNullOrEmpty(x.Follower) || !x.Follower.Split(';').Contains(userId.ToString()))
                .Select(x => x.UserId)
                .ToList();

            var uri = new Uri($"{_baseUrl}/v1/post-service/posts/get-creators");
            var creatorRequestData = new { CreatorIds = filterCreatorIds };
            var requestJsonData = JsonConvert.SerializeObject(creatorRequestData);

            return await GetCreatorFromPostServiceAsync(bearerToken, uri, requestJsonData);
        }
        private async Task<UserSimilarityRequest> GetUserSimilarityRequestAsnyc(PostgresDbContext db, Guid userId)
        {
            return await db.Users.Where(x => x.Id == userId).Include(x => x.UserDetail).Select(x => new UserSimilarityRequest
            {
                UserId = x.Id,
                Dob = x.UserDetail.Dob.Substring(0, 4),
                Sex = x.UserDetail.Sex,
                UserType = UserType.CONTENT_CREATOR
            }).FirstOrDefaultAsync();
        }

        private async Task<List<CreatorResponse>> GetDefaultCreatorsAsync(Guid userId)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var userImageRepository = _serviceProvider.GetRequiredService<IUserImageRepository>();

            var userEntities = await db.Users
                .Where(x => x.Id != userId && x.UserType == UserType.CONTENT_CREATOR)
                .Take(5)
                .Select(x => new { x.Id, x.UserName }).ToListAsync();
            var result = userEntities.Select(x => new CreatorResponse
            {
                UserId = x.Id,
                FullName = x.UserName
            }).ToList();

            var userIds = result?.Select(user => user.UserId).ToList();
            var userImages = await userImageRepository.GetListByUserIdAsync(userIds);

            return result.Select(user =>
            {
                var userImage = userImages?.FirstOrDefault(x => x.UserId == user.UserId);
                user.Avatar = userImage?.Url;
                return user;
            }).ToList();
        }

        private async Task<List<CreatorResponse>> GetCreatorFromPostServiceAsync(string bearerToken, Uri uri, string jsonData)
        {
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var responses = await _httpClientWrapper.PostAsync<List<CreatorResponse>>(uri.AbsoluteUri, content, new AuthConfiguration(bearerToken));

            return responses;
        }

        private async Task UpdateIsFollowerAsync(Guid userId, bool isFollower)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var currentUserDetail = await db.UserDetails.FirstOrDefaultAsync(x => x.UserId == userId);
            if (currentUserDetail is null)
            {
                _logger.LogError($"UseBalanceAsync: Not found user with Id ={userId}");
                throw new Exception($"Not found user with Id ={userId}");
            }
            currentUserDetail.IsFollower = isFollower;
            db.UserDetails.Update(currentUserDetail);
            await db.SaveChangesAsync();
        }

        private List<StatsticUserDonate> GetTopDonators(IEnumerable<UserTransaction> userTransferTransactions, PostgresDbContext dbContext)
        {
            var topDonators = userTransferTransactions.Where(x => x.FromUserId != null).GroupBy(x => (Guid)x.FromUserId);
            return GetStatisticalUserDonationList(topDonators, dbContext);
        }

        private List<StatsticUserDonate> GetTopReceivers(IEnumerable<UserTransaction> userTransferTransactions, PostgresDbContext dbContext)
        {
            var topReceivers = userTransferTransactions.GroupBy(x => x.ToUserId);
            return GetStatisticalUserDonationList(topReceivers, dbContext);
        }

        private List<StatsticUserDonate> GetStatisticalUserDonationList(IEnumerable<IGrouping<Guid, UserTransaction>> userTransactionGroups, PostgresDbContext dbContext)
        {
            var result = new List<StatsticUserDonate>();

            foreach (var group in userTransactionGroups.OrderByDescending(x => x.Sum(a => a.Amount)).Take(LeaderboardCount))
            {
                var userDetail = dbContext.UserDetails.Include(x => x.User).AsNoTracking().FirstOrDefault(x => x.User.Id == group.Key);
                if (userDetail != null)
                {
                    var userId = group.Key;
                    var total = group.Sum(x => x.Amount);
                    var avatar = userDetail.Avatar;
                    result.Add(new StatsticUserDonate()
                    {
                        Avatar = userDetail.Avatar,
                        TotalAmount = group.Sum(x => x.Amount),
                        UserId = group.Key,
                        UserName = userDetail.User.UserName
                    });
                }
            }

            return result;
        }

        private async Task<UserInfo> GetSystemActorInForAsync()
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            return await db.Users.Include(x => x.UserDetail)
                     .FirstOrDefaultAsync(x => x.FullName == "Hệ thống")
                     .Select(x => new UserInfo
                     (
                         userId: x.Id,
                         fullName: x.FullName,
                         avatar: x.UserDetail.Avatar
                     ));
        }

        #endregion
    }
}
