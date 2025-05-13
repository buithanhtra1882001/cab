using AutoMapper;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CabUserService.Constants;
using CabUserService.DomainCommands.Commands;
using CabUserService.Infrastructures.Constants;
using CabUserService.Infrastructures.DbContexts;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.IntegrationEvents.Events;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;
using CabUserService.Services.Base;
using CabUserService.Services.Interfaces;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CabUserService.Grpc.Profos.UserServer;
using CabUserService.Cqrs.Requests.Commands;

namespace CabUserService.Services
{
    public class ProfileService : BaseService<ProfileService>, IProfileService
    {
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppCache _cache;
        private readonly IUserService _userService;
        private readonly PostgresDbContext _postgresDbContext;

        public ProfileService(
            ILogger<ProfileService> logger,
            IServiceProvider serviceProvider,
            IMapper mapper,
            IAppCache cache,
            IUserService userService,
            PostgresDbContext postgresDbContext)
            : base(logger)
        {
            _mapper = mapper;
            _serviceProvider = serviceProvider;
            _cache = cache;
            _userService = userService;
            _postgresDbContext = postgresDbContext;
        }

        #region User
        public async Task<PublicUserInformationResponse> UserGetProfileAsync(Guid auid, Guid? cabUserId)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var userImageRepository = _serviceProvider.GetRequiredService<IUserImageRepository>();
            var requestReciveDonate = _postgresDbContext.DonateReceiverRequests.AsNoTracking().Include(x => x.User).FirstOrDefault(x => x.User.Id == auid);
            var userId = cabUserId.HasValue ? cabUserId : auid;
            var user = await userRepository.GetByIdAsync(userId);

            var userNotFound = user is null || user.IsSoftDeleted;
            if (userNotFound)
                return null;

            var userDetailRepository = _serviceProvider.GetRequiredService<IUserDetailRepository>();
            var userDetail = await userDetailRepository.GetByIdAsync(user.Id);

            var userMapping = userId == auid ?
                _mapper.Map<FullUserInformationResponse>(user) :
                _mapper.Map<PublicUserInformationResponse>(user);

            var response = _mapper.Map(userDetail, userMapping);
            response.IsCreateRequestReciveDonate = requestReciveDonate is null ? false : true;

            var avatar = await userImageRepository.GetByIdAsync(user.Id);
            if (avatar is not null)
                response.Avatar = avatar.Url;

            response.TotalFriend = await db.UserFriends.CountAsync(x => x.UserId == user.Id || x.FriendId == auid);
            response.IsFriend = await db.UserFriends
                .AnyAsync(x => (x.UserId == auid && x.FriendId == user.Id) || (x.UserId == user.Id && x.FriendId == auid));

            var currentUser = await db.Users.Where(x => x.Id == auid).Include(x => x.UserDetail).FirstOrDefaultAsync();
            response.IsFollow = string.Join(";", currentUser.UserDetail.Follower).Contains(user.Id.ToString());

            response.IsFriendRequest = await db.UserRequestFriendActions
                .AnyAsync(x => (x.UserId == auid && x.RequestUserId == user.Id)
                || (x.UserId == user.Id && x.RequestUserId == auid) && x.TypeRequest == REQUEST_TYPE.FRIEND
                && x.AcceptStatus == ACCEPTANCE_STATUS.NORMAL && x.StatusAction == ACTION_FRIEND_TYPE.SEND_REQUEST);

            response.CategoryFavorites = (from userCategory in _postgresDbContext.UserCategories 
                                          where userCategory.UserId == auid
                                          join categoryTable in _postgresDbContext.Categories
                                          on userCategory.CategoryId equals categoryTable.Id
                                         select new UserCategories
                                         {
                                             CategoryId = categoryTable.Id,
                                             CategoryName = categoryTable.Name
                                         }).Distinct().ToList();

            return response;
        }
        public async Task<List<PublicUserInformationResponse>> GetListUserProfileAsync(IEnumerable<Guid> auids)
        {
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
            var userImageRepository = _serviceProvider.GetRequiredService<IUserImageRepository>();
            var userDetailRepository = _serviceProvider.GetRequiredService<IUserDetailRepository>();
            List<PublicUserInformationResponse> result = new List<PublicUserInformationResponse>();
            //var userId = cabUserId.HasValue ? cabUserId : auid;
            // lấy ra danh sách users
            var users = await userRepository.GetListByIdAsync(auids.ToList());
            // lấy ra danh sách users detail 
            var userDeatails = await userDetailRepository.GetListDetailByIdAsync(auids.ToList());
            // lấy ra danh sách ảnh 
            var userImages = await userImageRepository.GetListByUserIdAsync(auids.ToList());
            foreach (var u in users)
            {
                var userMapping = _mapper.Map<PublicUserInformationResponse>(u);
                // lấy ra userdetail tương ứng 
                UserDetail foundUserDetail = userDeatails.FirstOrDefault(userDetail => userDetail.UserId == u.Id);
                PublicUserInformationResponse detailInfo = new PublicUserInformationResponse();
                if (null != foundUserDetail)
                {
                    detailInfo = _mapper.Map(foundUserDetail, userMapping);
                }
                // lấy ra avatar tương ứng 
                UserImage img = userImages.FirstOrDefault(userImage => userImage.UserId == u.Id);
                if (null != img)
                {
                    detailInfo.Avatar = img.Url;
                }
                result.Add(detailInfo);
            }
            return result;
        }

        public async Task UserCreateOrUpdateProfileAsync(Guid auid, UserCreateUpdateRequest userRequest)
        {
            var userDetailRepository = _serviceProvider.GetRequiredService<IUserDetailRepository>();

            var evenBus = _serviceProvider.GetRequiredService<IEventBus>();

            userRequest.UserId = auid;

            var userDetail = await userDetailRepository.GetByIdAsync(auid);
            if (userDetail is null)
            {
                await CreateProfileAsync(auid, userRequest.SequenceId, userRequest);
                evenBus.Publish(new UserProfileCreatedIntegrationEvent(auid, string.Empty, userRequest.Username, userRequest.Fullname));
            }
            else
            {
                await UpdateProfileAsync(userDetail, userRequest);
                evenBus.Publish(new UserProfileCreatedIntegrationEvent(auid, string.Empty, userRequest.Username, userRequest.Fullname));
                //evenBus.Publish(new UserProfileUpdatedIntegrationEvent(auid, userDetail.Avatar, userRequest.Username, userRequest.Fullname));
            }
        }

        public async Task<IEnumerable<UserFindByUserNameResponse>> UserGetListAsync(Guid auid, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

            var users = await userRepository.GetAllByUserName(auid, username);

            return _mapper.Map<IEnumerable<UserFindByUserNameResponse>>(users);

        }

        public async Task<string> UpdateAvatarAsync(Guid auid, string avatar, string url)
        {
            string result = string.Empty;
            try
            {
                var userDetailRepository = _serviceProvider.GetRequiredService<IUserDetailRepository>();
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var evenBus = _serviceProvider.GetRequiredService<IEventBus>();

                var userDetail = await userDetailRepository.GetByIdAsync(auid);
                if (userDetail is null)
                {
                    var user = await userRepository.GetByIdAsync(auid);
                    if (user is not null)
                    {
                        UserDetail userDetailModel = new UserDetail()
                        {
                            Avatar = avatar,
                            UserId = auid
                        };

                        await userDetailRepository.CreateAsync(userDetailModel);
                        evenBus.Publish(new UserProfileUpdateAvatarIntegrationEvent(userDetailModel.UserId, url));
                    }
                }
                else
                {
                    userDetail.Avatar = avatar;
                    await userDetailRepository.UpdateAsync(userDetail);
                    evenBus.Publish(new UserProfileUpdateAvatarIntegrationEvent(userDetail.UserId, url));
                }
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
            }
            return result;
        }

        public async Task<string> AddFollowerAsync(Guid userId, string followerId)
        {
            try
            {
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var user = await userRepository.GetByIdAsync(userId);
                var userNotFound = user is null || user.IsSoftDeleted;
                if (userNotFound)
                    return "User not exists";

                var userDetailRepository = _serviceProvider.GetRequiredService<IUserDetailRepository>();
                UserDetail userDetail = await userDetailRepository.GetUserDetailByUserIdAsync(userId);
                var userDetailNotFound = userDetail is null;
                if (userDetailNotFound)
                    return "User Detail not exists";

                if (string.IsNullOrEmpty(userDetail.Follower))
                {
                    await userDetailRepository.UpdateFollowerAsync(userId, followerId);

                }
                else
                {
                    if (userDetail.Follower.Split(";").Where(x => x == followerId).Count() == 0)
                    {
                        string follower = string.Format("{0};{1}", followerId, userDetail.Follower);
                        await userDetailRepository.UpdateFollowerAsync(userId, follower);
                    }
                    else
                    {
                        return "Follower exists";
                    }
                }

                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

                var userFollowHistory = new UserFollowHistory
                {
                    FollowerId = Guid.Parse(followerId),
                    UserId = userId
                };

                db.UserFollowHistories.Add(userFollowHistory);
                await db.SaveChangesAsync();

                string result = await AddFollowingAsync(Guid.Parse(followerId), userId.ToString());

                if (result.Length > 0)
                    return result;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public async Task<string> AddFollowingAsync(Guid userId, string followingId)
        {
            try
            {
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var user = await userRepository.GetByIdAsync(userId);
                var userNotFound = user is null || user.IsSoftDeleted;
                if (userNotFound)
                    return "User not exists";

                var userDetailRepository = _serviceProvider.GetRequiredService<IUserDetailRepository>();
                UserDetail userDetail = await userDetailRepository.GetUserDetailByUserIdAsync(userId);
                var userDetailNotFound = userDetail is null;
                if (userDetailNotFound)
                    return "User Detail not exists";

                if (string.IsNullOrEmpty(userDetail.Following))
                {
                    await userDetailRepository.UpdateFollowingAsync(userId, followingId);
                }
                else
                {
                    if (userDetail.Following.Split(";").Where(x => x == followingId).Count() == 0)
                    {
                        string follower = string.Format("{0};{1}", followingId, userDetail.Follower);
                        await userDetailRepository.UpdateFollowingAsync(userId, follower);
                    }
                    else
                    {
                        return "Following exists";
                    }
                }

                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

                var userFollowHistory = new UserFollowHistory
                {
                    FollowerId = userId,
                    UserId = Guid.Parse(followingId)
                };

                db.UserFollowHistories.Add(userFollowHistory);
                await db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return string.Empty;
        }

        public async Task<string> UnfollowerAsync(Guid userId, Guid followerId)
        {
            try
            {
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var user = await userRepository.GetByIdAsync(userId);
                var userNotFound = user is null || user.IsSoftDeleted;
                if (userNotFound)
                    return "User not exists";

                var userDetailRepository = _serviceProvider.GetRequiredService<IUserDetailRepository>();
                var userDetail = await userDetailRepository.GetUserDetailByUserIdAsync(userId);
                var userDetailNotFound = userDetail is null;
                if (userDetailNotFound)
                    return "User Detail not exists";

                if (string.IsNullOrEmpty(userDetail.Follower))
                    return $"User with Id = {userId} has no followers";

                var followerIds = userDetail.Follower?.Split(";").ToList();

                if (!followerIds.Contains(followerId.ToString()))
                {
                    return $"Follower with Id = {followerId} not found";
                }

                followerIds.Remove(followerId.ToString());

                var newFollowerIds = string.Join(";", followerIds);
                await userDetailRepository.UpdateFollowerAsync(userDetail.UserId, newFollowerIds);

                var result = await UnfollowingAsync(followerId, userId);
                if (result.Length > 0)
                    return result;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
            return string.Empty;
        }

        public async Task<string> UnfollowingAsync(Guid userId, Guid followingId)
        {
            try
            {
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();
                var user = await userRepository.GetByIdAsync(userId);
                var userNotFound = user is null || user.IsSoftDeleted;
                if (userNotFound)
                    return "User not exists";

                var userDetailRepository = _serviceProvider.GetRequiredService<IUserDetailRepository>();
                var userDetail = await userDetailRepository.GetUserDetailByUserIdAsync(userId);
                var userDetailNotFound = userDetail is null;
                if (userDetailNotFound)
                    return "User Detail not exists";

                if (string.IsNullOrEmpty(userDetail.Following))
                    return $"User with Id = {userId} has no followers";

                var followingIds = userDetail.Following?.Split(";").ToList();

                if (!followingIds.Contains(followingId.ToString()))
                {
                    return $"Follower with Id = {followingId} not found";
                }

                followingIds.Remove(followingId.ToString());

                var newFollowingIds = string.Join(";", followingIds);
                await userDetailRepository.UpdateFollowingAsync(userDetail.UserId, newFollowingIds);

                return "Unfollowing and Unfollower successfull";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<TotalFollowResponse> GetTotalFollowAsync(Guid userId)
        {
            TotalFollowResponse totalFollowResponse = new TotalFollowResponse();
            totalFollowResponse.TotalFollower = 0;
            totalFollowResponse.TotalFollowing = 0;
            try
            {
                var userDetailRepository = _serviceProvider.GetRequiredService<IUserDetailRepository>();
                UserDetail userDetail = await userDetailRepository.GetUserDetailByUserIdAsync(userId);
                var userDetailNotFound = userDetail is null;
                if (userDetailNotFound)
                {
                    totalFollowResponse.Message = "User Detail not exists";
                    return totalFollowResponse;
                }

                if (!string.IsNullOrEmpty(userDetail.Follower))
                    totalFollowResponse.TotalFollower = TotalFollower(userDetail).Count();

                if (!string.IsNullOrEmpty(userDetail.Following))
                    totalFollowResponse.TotalFollowing = TotalFollowing(userDetail).Count();
            }
            catch (Exception ex)
            {
                totalFollowResponse.Message = ex.Message;
            }
            return totalFollowResponse;
        }

        public async Task<Response> GetFollowListAsync(Guid userId, FOLLOW_TYPE type)
        {
            Response response = new Response() { Data = null, Message = string.Empty, StatusCode = 200 };
            try
            {
                var userDetailRepository = _serviceProvider.GetRequiredService<IUserDetailRepository>();
                UserDetail userDetail = await userDetailRepository.GetUserDetailByUserIdAsync(userId);
                var userDetailNotFound = userDetail is null;
                if (userDetailNotFound)
                {
                    response.Message = "User Detail not exists";
                    response.StatusCode = 400;
                    return response;
                }

                if (type == FOLLOW_TYPE.FOLLOWER)
                {
                    response.Data = TotalFollower(userDetail);
                }
                else
                {
                    response.Data = TotalFollowing(userDetail);
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.StatusCode = 500;
            }
            return response;
        }

        public async Task<bool> ViewUserProfileAsync(Guid viewerId, Guid profileUserId)
        {
            if (viewerId.ToString().ToLower() == profileUserId.ToString().ToLower())
            {
                _logger.LogInformation("No save count view by yourselft");
                return false;
            }

            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            var viewerEntity = await db.Users.FirstOrDefaultAsync(x => x.Id == viewerId);
            if (viewerEntity is null)
                return false;

            var profileUserEntity = await db.Users.FirstOrDefaultAsync(x => x.Id == profileUserId);
            if (profileUserEntity is null)
                return false;

            var userViewProfileHistory = new UserViewProfileHistory
            {
                Id = Guid.NewGuid(),
                OwnerId = profileUserId,
                ViewerId = viewerId
            };

            db.UserViewProfileHistories.Add(userViewProfileHistory);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<long> StatisticsProfileViewAsync(Guid auid, IntervalType intervalType)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            var now = DateTime.UtcNow;

            DateTime startDateOfWeek = now.AddDays(-(int)now.DayOfWeek);
            DateTime endDateOfWeek = startDateOfWeek.AddDays(6);

            DateTime startDateOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime endDateOfMonth = startDateOfMonth.AddMonths(1).AddDays(-1);

            DateTime startDateOfYear = new DateTime(now.Year, 1, 1);
            DateTime endDateOfYear = new DateTime(now.Year, 12, 31);

            return intervalType switch
            {
                IntervalType.WEEK => await db.UserViewProfileHistories
                                        .CountAsync(x => x.OwnerId == auid && x.CreatedAt >= startDateOfWeek && x.CreatedAt <= endDateOfWeek),
                IntervalType.MONTH => await db.UserViewProfileHistories
                                        .CountAsync(x => x.OwnerId == auid && x.CreatedAt >= startDateOfMonth && x.CreatedAt <= endDateOfMonth),
                IntervalType.YEAR => await db.UserViewProfileHistories
                                        .CountAsync(x => x.OwnerId == auid && x.CreatedAt >= startDateOfYear && x.CreatedAt <= endDateOfYear),
                _ => (long)0,
            };
        }

        public async Task<InteractionStatisticsResponse> InteractionStatisticsAsync(Guid auid, IntervalType intervalType)
        {
            var cacheKey = $"InteractionStatisticsAsync_{auid}_{intervalType}";

            Task<InteractionStatisticsResponse> objectFactory() => InteractionStatisticsHanlderAsync(auid, intervalType);

            return (await _cache.GetOrAddAsync(
                cacheKey,
                objectFactory, 
                DateTimeOffset.Now.AddMinutes(5)));
        }

        public async Task<string> UpdateBackgroundCoverAsync(Guid auid, string avatar, string url)
        {
            string result = string.Empty;
            try
            {
                var userDetailRepository = _serviceProvider.GetRequiredService<IUserDetailRepository>();
                var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

                var userDetail = await userDetailRepository.GetByIdAsync(auid);
                if (userDetail is null)
                {
                    return "Tài khoản không tồn tại";
                }
                else
                {
                    userDetail.CoverImage = url;
                    await userDetailRepository.UpdateAsync(userDetail);
                }
            }

            catch (Exception ex)
            {
                result = ex.Message.ToString();
            }
            return result;
        }

        #endregion

        #region Admin
        public async Task<PagingResponse<User>> AdminGetListAsync(GetAllUserRequest request)
        {
            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

            var total = await userRepository.GetTotalUser(request);
            var data = await userRepository.GetAllAsync(request);

            return new PagingResponse<User>
            {
                Total = total,
                Data = data
            };
        }

        public async Task<PublicUserInformationResponse> AdminGetProfileAsync(Guid auid, string userRole)
        {
            var userDetailRepository = _serviceProvider.GetRequiredService<IUserDetailRepository>();

            var userDetail = await userDetailRepository.GetByIdAsync(auid);

            if (userRole == UserRoleConstant.MOD)
                return _mapper.Map<PublicUserInformationResponse>(userDetail);
            else
                return _mapper.Map<FullUserInformationResponse>(userDetail);
        }

        public async Task<string> AdminDeleteUserAsync(Guid auid, Guid cabUserId)
        {
            string deletedUserId = null;

            var userRepository = _serviceProvider.GetRequiredService<IUserRepository>();

            var user = await userRepository.GetByIdAsync(cabUserId);
            if (user != null && !user.IsSoftDeleted && auid != cabUserId)
            {
                var deleteUserCommand = new DeleteUserCommand
                {
                    Id = cabUserId
                };

                var evenBus = _serviceProvider.GetRequiredService<IEventBus>();
                var mediator = _serviceProvider.GetRequiredService<IMediator>();
                await mediator.Publish(deleteUserCommand);
                evenBus.Publish(new UserDeletedIntegrationEvent(user.Email));
                deletedUserId = user.Id.ToString();
            }

            return deletedUserId;
        }
        #endregion

        #region Private Methods
        private async Task CreateProfileAsync(Guid auid, int id, UserCreateUpdateRequest userRequest)
        {
            var mediator = _serviceProvider.GetRequiredService<IMediator>();

            var userDetail = _mapper.Map<UserDetail>(userRequest);

            userDetail.UserId = auid;
            userDetail.Avatar = string.Empty;

            var createUserDetailCommand = new CreateUserDetailCommand
            {
                UserDetail = userDetail,
                SequenceId = id
            };

            await mediator.Publish(createUserDetailCommand);
        }

        private async Task UpdateProfileAsync(UserDetail userDetail, UserCreateUpdateRequest userRequest)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var mediator = _serviceProvider.GetRequiredService<IMediator>();

            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == userRequest.UserId);

            if (user is null)
            {
                _logger.LogError($"Not found user with Id={userRequest.UserId}");
                throw new InvalidOperationException($"Not found user with Id={userRequest.UserId}");
            }

            user.FullName = userRequest.Fullname;
            user.UserName = userRequest.Username;

            db.Users.Update(user);
            await db.SaveChangesAsync();

            userDetail.Dob = userRequest.Dob;
            userDetail.City = userRequest.City;
            userDetail.Phone = userRequest.Phone;
            userDetail.Sex = userRequest.Sex;
            userDetail.IdentityCardNumber = userRequest.IdentityCardNumber;
            userDetail.Description = userRequest.Description;
            userDetail.UpdatedAt = DateTime.UtcNow;
            userDetail.IsUpdateProfile = true;

            var updateUserDetailCommand = new UpdateUserDetailCommand
            {
                UserDetail = userDetail
            };

            await mediator.Publish(updateUserDetailCommand);
            await _userService.UpdateProfileAsync(new UpdateProfileCommand()
            {
                UserId = userDetail.UserId,
                SexualOrientation = userRequest.SexualOrientation,
                Marry = userRequest.Marry,
                CategoryFavorites = userRequest.CategoryFavorites,
                Schools = userRequest.Schools,
                Companys = userRequest.Companys,
                HomeLand = userRequest.HomeLand,
                IsShowSexualOrientation = userRequest.IsShowSexualOrientation,
                IsShowMarry = userRequest.IsShowMarry,
                IsShowSchool = userRequest.IsShowSchool,
                IsShowCompany = userRequest.IsShowCompany,
                IsShowHomeLand = userRequest.IsShowHomeLand,
                IsShowDob = userRequest.IsShowDob,
                IsShowEmail = userRequest.IsShowEmail,
                IsShowPhone = userRequest.IsShowPhone,
                IsShowCity = userRequest.IsShowCity,
                IsShowDescription = userRequest.IsShowDescription
            });
        }

        private static string[] TotalFollower(UserDetail userDetail)
        {
            try
            {
                return !string.IsNullOrEmpty(userDetail.Follower) ? userDetail.Follower.Split(";") : null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static string[] TotalFollowing(UserDetail userDetail)
        {
            try
            {
                return !string.IsNullOrEmpty(userDetail.Following) ? userDetail.Following.Split(";") : null;
            }
            catch
            {
                return null;
            }
        }

        private async Task<long> StatisticsTotalFollowAsync(Guid auid, IntervalType intervalType)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            var now = DateTime.UtcNow;

            DateTime startDateOfWeek = now.AddDays(-(int)now.DayOfWeek);
            DateTime endDateOfWeek = startDateOfWeek.AddDays(6);

            DateTime startDateOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime endDateOfMonth = startDateOfMonth.AddMonths(1).AddDays(-1);

            DateTime startDateOfYear = new DateTime(now.Year, 1, 1);
            DateTime endDateOfYear = new DateTime(now.Year, 12, 31);

            return intervalType switch
            {
                IntervalType.WEEK => await db.UserFollowHistories
                                        .CountAsync(x => x.UserId == auid && x.CreatedAt >= startDateOfWeek && x.CreatedAt <= endDateOfWeek),
                IntervalType.MONTH => await db.UserFollowHistories
                                        .CountAsync(x => x.UserId == auid && x.CreatedAt >= startDateOfMonth && x.CreatedAt <= endDateOfMonth),
                IntervalType.YEAR => await db.UserFollowHistories
                                        .CountAsync(x => x.UserId == auid && x.CreatedAt >= startDateOfYear && x.CreatedAt <= endDateOfYear),
                _ => (long)0,
            };
        }

        private async Task<InteractionStatisticsResponse> InteractionStatisticsHanlderAsync(
            Guid auid,
            IntervalType intervalType)
        {
            var response = new InteractionStatisticsResponse
            {
                TotalProfileView = await StatisticsProfileViewAsync(auid, intervalType)
            };

            var balanceService = _serviceProvider.GetRequiredService<IBalanceService>();

            response.TotalDonationAmount = await balanceService.GetTotalDonateAsync(auid, new TotalDonateRequest
            {
                IntervalType = intervalType
            });

            response.TotalFollow = await StatisticsTotalFollowAsync(auid, intervalType);
            return response;
        }
        
        #endregion
    }
}