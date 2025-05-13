using AutoMapper;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CabNotificationService.Constants;
using CabPostService.Models.Dtos;
using CabUserService.Constants;
using CabUserService.Hubs;
using CabUserService.Infrastructures.DbContexts;
using CabUserService.Infrastructures.Helper;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.IntegrationEvents.Events;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;
using CabUserService.Services.Base;
using CabUserService.Services.Interfaces;
using MassTransit.Initializers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http.Headers;

namespace CabUserService.Services
{
    public class BalanceService :
        BaseService<BalanceService>,
        IBalanceService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;
        private readonly IHubContext<LiveDonationNotificationHub> _hubContext;
        private readonly IConfiguration _configuration;

        public BalanceService(ILogger<BalanceService> logger,
            IServiceProvider serviceProvider,
            IMapper mapper,
            IHubContext<LiveDonationNotificationHub> hubContext,
            IConfiguration configuration)
            : base(logger)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _hubContext = hubContext;
            _configuration = configuration;
        }

        public async Task<Guid?> UserDonataionAsync(Guid auid, UserDonationRequest request, Guid? postId)
        {
            CommonHelper.ProtectAction("Donataion", auid.ToString(), 10);

            _logger.LogInformation($"UserDonataionAsync: request={JsonConvert.SerializeObject(request)}");

            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            var fromUser = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == auid);
            var toUser = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.ToUserId);

            ValidationDonationAsync(fromUser, toUser, request);

            var isPostDonate = postId != null;
            if (isPostDonate)
            {
                await ValidatePost((Guid)postId, request.ToUserId);
            }

            using var transaction = db.Database.BeginTransaction();

            try
            {
                await UseBalanceAsync(new UseBalanceRequest
                {
                    Amount = request.Amount,
                    UserId = auid,
                    Type = BalanceType.DONATAION,
                    Description = $"Donate for user: {request.ToUserId}",
                }, db);

                var platformFee = request.Amount * CommonConstant.PlatformDonationFee;
                var actualRecievedAmount = request.Amount - platformFee;

                await AddBalanceAsync(new AddBalanceRequest
                {
                    Amount = actualRecievedAmount,
                    UserId = request.ToUserId,
                    Type = BalanceType.DONATAION,
                    Description = $"Receive donate from user: {auid}",
                }, db);

                var donationText = string.Join("", $"User: {auid} donate for user: {request.ToUserId}, amount = {request.Amount}", isPostDonate ? $" through post: {postId.ToString()}" : string.Empty);

                var logId = await AddUserTransactionAsync(
                    new AddUserTransactionRequest
                    {
                        Amount = actualRecievedAmount,
                        FromUserId = auid,
                        ToUserId = request.ToUserId,
                        Type = TransactionType.TRANSFER,
                        Status = TransactionStatus.SUCCESS,
                        DonationType = isPostDonate ? DonateType.PostDonate : DonateType.NormalDonate,
                        PostId = postId,
                        Description = donationText,
                        DonationMessage = request.Message,
                        PlatformFee = platformFee,
                    }, db);

                await transaction.CommitAsync();

                Log.ForContext("donatation", $"{auid}").Information(donationText);

                // Send donation info to SignalR clients, only Content Creator can get notification, donation through posts is ignored
                var eventBus = _serviceProvider.GetRequiredService<IEventBus>();
                var userImageRepository = _serviceProvider.GetRequiredService<IUserImageRepository>();

                var userImg = await userImageRepository.GetByIdAsync(fromUser.Id);

                var actorInfor = new UserInfo
                    (
                    userId: fromUser.Id,
                    fullName: fromUser.FullName,
                    avatar: userImg?.Url
                    );

                var systemInfor = await GetSystemActorInForAsync();

                if (toUser.UserType == UserType.CONTENT_CREATOR && !isPostDonate)
                {
                    eventBus.Publish(new NotificationIntegrationEvent
                        (
                         userIds: new List<Guid> { toUser.Id },
                         actor: actorInfor,
                         referenceId: fromUser.Id,
                         notificationType: NotificationConstant.DonateCreator,
                         donateAmount: request.Amount,
                         referenceUrl: null,
                         type: null
                         ));

                     eventBus.Publish(new NotificationIntegrationEvent
                        (
                         userIds: new List<Guid> { toUser.Id },
                         actor: systemInfor,
                         referenceId: fromUser.Id,
                         notificationType: NotificationConstant.SystemDonateCreator,
                         donateAmount: request.Amount,
                         referenceUrl: null,
                         type: NotificationConstant.System
                         ));
                    //var donationInfoPayload = new DonationInfoPayload(fromUser.UserName, request.Amount, request.Message, string.Empty);
                    //await SendDonationInfoAsync(toUser.Id.ToString(), donationInfoPayload);
                }
                else
                {
                    eventBus.Publish(new NotificationIntegrationEvent
                        (
                         userIds: new List<Guid> { toUser.Id },
                         actor: actorInfor,
                         referenceId: postId ?? Guid.Empty,
                         notificationType: NotificationConstant.DonatePost,
                         donateAmount: request.Amount,
                         referenceUrl: null,
                         type: null
                         ));

                    eventBus.Publish(new NotificationIntegrationEvent
                        (
                         userIds: new List<Guid> { toUser.Id },
                         actor: systemInfor,
                         referenceId: postId ?? Guid.Empty,
                         notificationType: NotificationConstant.SystemDonatePost,
                         donateAmount: request.Amount,
                         referenceUrl: null,
                         type: NotificationConstant.System
                         ));
                }

                return logId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"UserDonataionAsync: Error={ex.Message}");

                if (transaction.GetDbTransaction() is not null)
                    await transaction.RollbackAsync();

                throw new Exception(ex.Message);
            }
        }

        public async Task<string> GetDonateURLParam(Guid auid)
        {
            try
            {
                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
                var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == auid);

                if (user is null)
                {
                    throw new Exception($"User with ID {auid} not found");
                }

                if (user.UserType != UserType.CONTENT_CREATOR)
                {
                    throw new Exception($"User with ID {auid} is not a Content Creator");
                }

                return auid.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"UserDonataionAsync: Error={ex.Message}");
                return null;
            }
        }

        public async Task<PagingResponse<UserBalanceLogDto>> GetUserBalanceLogAsync(
            Guid auid,
            GetUserBalanceLogRequest request)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            var query = db.UserBalanceLogs
                .AsNoTracking()
                .Where(x => x.UserId == auid);

            var count = await query.CountAsync();
            var items = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var resultItems = _mapper.Map<List<UserBalanceLogDto>>(items);

            return new PagingResponse<UserBalanceLogDto>
            {
                Data = resultItems,
                Total = count
            };
        }

        public async Task<PagingResponse<UserTransactionDto>> GetUserTransactionLogAsync(
            Guid auid,
            GetUserTransactionRequest request)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            var query = db.UserTransactionLogs
                .AsNoTracking()
                .Where(x => x.FromUserId == auid || x.ToUserId == auid);

            if (request.Type.HasValue)
                query = query.Where(x => x.Type == request.Type);

            var count = await query.CountAsync();
            var items = query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var resultItems = _mapper.Map<List<UserTransactionDto>>(items);

            return new PagingResponse<UserTransactionDto>
            {
                Data = resultItems,
                Total = count
            };
        }

        public async Task<UserTransactionDto> GetUserTransactionByIdAsync(
            Guid auid,
            GetUserTransactionByIdRequest request)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            var entity = await db.UserTransactionLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                                     (x.FromUserId == auid || x.ToUserId == auid) &&
                                     x.Id == request.Id);

            return _mapper.Map<UserTransactionDto>(entity);
        }

        public async Task<double> GetTotalDonnateAsync(Guid auid, TotalDonateRequest request)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var donateQuery = db.UserBalanceLogs.AsNoTracking().Where(x => x.UserId == auid && x.Type == BalanceType.DONATAION);
            double totalAmount = 0;
            var now = DateTime.UtcNow;
            switch (request.IntervalType)
            {
                case IntervalType.WEEK:
                    totalAmount = await donateQuery.Where(x => x.CreatedAt >= now.AddDays(-7)).SumAsync(x => x.Amount);
                    break;
                case IntervalType.MONTH:
                    totalAmount = await donateQuery.Where(x => x.CreatedAt >= now.AddMonths(-1)).SumAsync(x => x.Amount);
                    break;
                case IntervalType.YEAR:
                    totalAmount = await donateQuery.Where(x => x.CreatedAt >= now.AddYears(-1)).SumAsync(x => x.Amount);
                    break;
                default:
                    _logger.LogError($"Invalid {request.IntervalType} interval");
                    throw new Exception("Invalid interval");
            }
            return totalAmount;
        }

        public async Task<double> GetTotalDonateAsync(Guid auid, TotalDonateRequest request)
        {
            var isCreator = await CheckCreatorAsync(auid);

            if (!isCreator)
            {
                _logger.LogError("User cannot create");
                throw new InvalidOperationException("User is not a content creator");
            }
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var donateQuery = db.UserBalanceLogs.AsNoTracking().Where(x => x.UserId == auid && x.Type == BalanceType.DONATAION);
            double totalAmount = 0;
            var now = DateTime.UtcNow;
            switch (request.IntervalType)
            {
                case IntervalType.WEEK:
                    totalAmount = await donateQuery.Where(x => x.CreatedAt >= now.AddDays(-7)).SumAsync(x => x.Amount);
                    break;
                case IntervalType.MONTH:
                    totalAmount = await donateQuery.Where(x => x.CreatedAt >= now.AddMonths(-1)).SumAsync(x => x.Amount);
                    break;
                case IntervalType.YEAR:
                    totalAmount = await donateQuery.Where(x => x.CreatedAt >= now.AddYears(-1)).SumAsync(x => x.Amount);
                    break;
                default:
                    _logger.LogError($"Invalid {request.IntervalType} interval");
                    throw new Exception("Invalid interval");
            }
            return totalAmount;
        }

        public async Task<List<DonatorResponse>> GetTopDonatorAsync(Guid auid, TotalDonateRequest request)
        {
            var isCreator = await CheckCreatorAsync(auid);

            if (!isCreator)
            {
                _logger.LogError("User cannot create");
                throw new InvalidOperationException("User is not a content creator");
            }
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var now = DateTime.UtcNow;
            DateTime startDate;
            switch (request.IntervalType)
            {
                case IntervalType.WEEK:
                    startDate = now.AddDays(-7);
                    break;
                case IntervalType.MONTH:
                    startDate = now.AddMonths(-1);
                    break;
                case IntervalType.YEAR:
                    startDate = now.AddYears(-1);
                    break;
                default:
                    _logger.LogError($"Invalid {request.IntervalType} interval");
                    throw new Exception("Invalid interval");
            }
            var topDonators = await db.UserBalanceLogs.AsNoTracking()
                                                   .Where(x => x.UserId == auid && x.Type == BalanceType.DONATAION && x.CreatedAt >= startDate)
                                                   .Join(db.Users, userBalanceLog => userBalanceLog.UserId, user => user.Id, (userBalanceLog, user)
                                                   => new
                                                   {
                                                       UserId = userBalanceLog.UserId,
                                                       UserName = user.UserName,
                                                       Amount = userBalanceLog.Amount,
                                                   })
                                                   .GroupBy(x => x.UserId)
                                                   .Select(g => new DonatorResponse
                                                   {
                                                       UserName = g.First().UserName,
                                                       TotalAmount = g.Sum(x => x.Amount)
                                                   }).OrderByDescending(x => x.TotalAmount).Take(10).ToListAsync();
            return topDonators;
        }

        public async Task<bool> CreatorHideOrShowDonationContentAsync(Guid auid, Guid transactionId)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            var isCreatorUser = await CheckCreatorAsync(auid);

            if (!isCreatorUser)
                throw new Exception("Your are not allow to use this action.");

            var transactionEntity = await db.UserTransactionLogs
                .FirstOrDefaultAsync(x =>
                                     x.ToUserId == auid &&
                                     x.Id == transactionId &&
                                     x.Type == TransactionType.TRANSFER);

            if (transactionEntity is null)
            {
                _logger.LogError($"CreatorHideOrShowDonationContentAsync: This transaction is invalid.");
                throw new Exception("This transaction is invalid.");
            }

            transactionEntity.IsHidingMessage = !transactionEntity.IsHidingMessage;

            db.UserTransactionLogs.Update(transactionEntity);
            await db.SaveChangesAsync();

            return true;
        }

        #region Private Methods

        private async Task ValidatePost(Guid postId, Guid postOnwerId)
        {
            var baseAddress = _configuration["PostService:BaseAddress"];
            string getPostByIdUrl = string.Empty;

            // In production, post url is different from development
            if (baseAddress.Contains("localhost"))
            {
                getPostByIdUrl = $"{baseAddress}/api/v1/posts/{postId.ToString()}";
            }
            else
            {
                getPostByIdUrl = $"{_configuration["PostService:BaseAddress"]}/v1/post-service/posts/{postId.ToString()}";
            }

            UserPostResponse result = null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.GetAsync(getPostByIdUrl);
                    var content = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<UserPostResponse>(content);
                }
            }
            catch
            {
                throw new Exception("Error reaching Post endpoint");
            }

            if (result is null)
            {
                throw new Exception($"Post: {postId.ToString()} not found");
            }

            if (postOnwerId != result.UserId)
            {
                throw new Exception($"Post and post onwer mismatch");
            }
        }

        private void ValidationDonationAsync(User fromUser, User toUser, UserDonationRequest request)
        {
            if (request is null)
                throw new Exception("Request is null");

            if (fromUser == toUser)
            {
                _logger.LogError($"UserDonataionAsync: Can't donate by yourself.");
                throw new Exception("Can't donate by yourself.");
            }

            if (request.Amount < CommonConstant.DonateMinValue)
            {
                _logger.LogError($"UserDonataionAsync: Donate amount < {CommonConstant.DonateMinValue}");
                throw new Exception($"Donate at least {CommonConstant.DonateMinValue}");
            }

            if (fromUser is null)
                throw new Exception($"Not found info from userId={fromUser.Id}");

            if (toUser is null)
                throw new Exception($"Not found info from receive userId={request.ToUserId}");

            //if (toUser.UserType != UserType.CONTENT_CREATOR)
            //    throw new Exception($"Can't not donate for this user, type is invalid userType={toUser.UserType}");

            if (!toUser.CanReceiveDonation)
                throw new Exception($"User {toUser.Id} is not allowed to receive donation");
        }

        private async Task<Guid> AddBalanceAsync(
            AddBalanceRequest request,
            PostgresDbContext db = null)
        {
            db ??= _serviceProvider.GetRequiredService<PostgresDbContext>();

            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user is null)
            {
                _logger.LogError($"AddBalanceAsync: Not found user with Id ={request.UserId}");
                throw new Exception($"Not found user with Id ={request.UserId}");
            }

            var prevAmount = user.Coin;
            user.Coin += request.Amount;
            user.UpdatedAt = DateTime.UtcNow;

            db.Users.Update(user);
            await db.SaveChangesAsync();

            var logid = await AddUserBalanceLogAsync(
                user.Id,
                prevAmount,
                request.Amount,
                request.Description,
                request.Type,
                db);

            return logid;
        }

        public async Task<Guid> UseBalanceAsync(
            UseBalanceRequest request,
            PostgresDbContext db = null)
        {
            db ??= _serviceProvider.GetRequiredService<PostgresDbContext>();

            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user is null)
            {
                _logger.LogError($"UseBalanceAsync: Not found user with Id ={request.UserId}");
                throw new Exception($"Not found user with Id ={request.UserId}");
            }

            var balanceAmount = user.Coin;

            if (balanceAmount < request.Amount)
            {
                _logger.LogError($"UseBalanceAsync: Balance={balanceAmount} < Amount={request.Amount}");
                throw new Exception("Balance < Amount");
            }

            var prevAmount = balanceAmount;
            user.Coin = balanceAmount - request.Amount;
            user.UpdatedAt = DateTime.UtcNow;

            db.Users.Update(user);
            await db.SaveChangesAsync();

            var logid = await AddUserBalanceLogAsync(
                user.Id,
                prevAmount,
                0 - request.Amount,
                request.Description,
                request.Type,
                db);

            return logid;
        }

        private async Task<Guid> AddUserBalanceLogAsync(
            Guid userId,
            double prevAmount,
            double addAmount,
            string description,
            BalanceType type,

            PostgresDbContext db = null)
        {
            db ??= _serviceProvider.GetRequiredService<PostgresDbContext>();

            var entity = new UserBalanceLog
            {
                Amount = addAmount,
                CreatedAt = DateTime.UtcNow,
                Description = $"{description}. Prev-amount:{prevAmount}",
                Type = type,
                Id = Guid.NewGuid(),
                UserId = userId,
            };

            db.UserBalanceLogs.Add(entity);
            await db.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<Guid> AddUserTransactionAsync(
            AddUserTransactionRequest request,
            PostgresDbContext db = null)
        {
            db ??= _serviceProvider.GetRequiredService<PostgresDbContext>();

            var entity = _mapper.Map<UserTransaction>(request);

            db.UserTransactionLogs.Add(entity);
            await db.SaveChangesAsync();

            return entity.Id;
        }

        private async Task<bool> CheckCreatorAsync(Guid auid)
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var isCreator = await db.Users
                .Where(x => x.Id == auid && x.UserType == UserType.CONTENT_CREATOR)
                .AnyAsync();

            return isCreator;
        }

        /// <summary>
        /// Broadcast donate notification to streaming tool browser
        /// </summary>
        /// <param name="recieverDonateId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private async Task SendDonationInfoAsync(string recieverDonateId, DonationInfoPayload payload)
        {
            try
            {
                var connections = LiveDonationNotificationHub.LiveDonationConnections.GetConnections(recieverDonateId);

                if (!connections.Any())
                    return;

                foreach (var connectionId in connections)
                {
                    var client = _hubContext.Clients.Client(connectionId);
                    if (client != null)
                    {
                        await client.SendAsync("RecieveDonationInfo", payload);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"SendDonationInfoAsync: Failed to broadcast donation info - Error={ex.Message}");
            }
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