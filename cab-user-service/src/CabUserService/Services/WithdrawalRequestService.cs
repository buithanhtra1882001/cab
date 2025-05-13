using AutoMapper;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CabNotificationService.Constants;
using CabUserService.Constants;
using CabUserService.Infrastructures.DbContexts;
using CabUserService.IntegrationEvents.Events;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;
using CabUserService.Services.Base;
using CabUserService.Services.Interfaces;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CabUserService.Services
{
    public class WithdrawalRequestService : BaseService<WithdrawalRequestService>, IWithdrawalRequestService
    {
        private IServiceProvider _serviceProvider;

        private readonly IMapper _mapper;

        private readonly IBalanceService _balanceService;

        private readonly PostgresDbContext _dbContext;

        public WithdrawalRequestService(
            ILogger<WithdrawalRequestService> logger,
            IServiceProvider serviceProvider,
            IMapper mapper,
            PostgresDbContext postgresDbContext,
            IBalanceService balanceService)
        : base(logger)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _dbContext = postgresDbContext;
            _balanceService = balanceService;
        }

        public async Task<WithdrawalRequestResponse> CreateRequestAsync(Guid userId, WithdrawalRequestDto requestDto)
        {
            ValidateUser(userId, out var user);

            var requestAlreadyExists = await GetPendingRequestAsync(userId) != null;
            if (requestAlreadyExists)
            {
                throw new Exception($"Pending request of user {userId} already exists - can have only one pending request");
            }

            if (!user.IsVerifyEmail)
            {
                throw new Exception($"User {userId} is not allowed to create withdrawal request, email verification needed");
            }

            if (user.Coin < requestDto.WithdrawalAmount)
            {
                throw new Exception($"User {userId} doesn't have enough coin");
            }

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var newRequest = _mapper.Map<WithdrawalRequest>(requestDto);

                    newRequest.Id = new Guid();
                    newRequest.User = user;
                    newRequest.Status = WithdrawalRequestStatus.Pending;
                    newRequest.UpdatedAt = DateTime.Now;
                    newRequest.CreatedAt = DateTime.Now;

                    _dbContext.WithdrawalRequests.Add(newRequest);

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var eventBus = _serviceProvider.GetRequiredService<IEventBus>();

                    var actorInfo = await GetSystemActorInForAsync();

                    eventBus.Publish(new NotificationIntegrationEvent
                        (
                         userIds: new List<Guid> { user.Id },
                         actor: actorInfo,
                         referenceId: user.Id,
                         notificationType: NotificationConstant.CreateWithdrawalRequest,
                         donateAmount: newRequest.WithdrawalAmount,
                         referenceUrl: null,
                         type: NotificationConstant.System
                         ));

                    var response = _mapper.Map<WithdrawalRequestResponse>(newRequest);
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"WithdrawalRequestService.CreateRequestAsync: Error={ex.Message}");

                    if (transaction.GetDbTransaction() is not null)
                        await transaction.RollbackAsync();

                    return null;
                }
            }
        }

        public async Task<WithdrawalRequestResponse> GetPendingRequestAsync(Guid auid)
        {
            var request = _dbContext.WithdrawalRequests.AsNoTracking().Include(x => x.User).FirstOrDefault(x => x.User.Id == auid && x.Status == WithdrawalRequestStatus.Pending);
            return request is null ? null : _mapper.Map<WithdrawalRequestResponse>(request);
        }

        public async Task<WithdrawalRequestResponse> GetAllPendingRequestsAsync()
        {
            var request = _dbContext.WithdrawalRequests.AsNoTracking().Include(x => x.User).FirstOrDefault(x => x.Status == WithdrawalRequestStatus.Pending);
            return request is null ? null : _mapper.Map<WithdrawalRequestResponse>(request);
        }

        public async Task<List<WithdrawalRequestResponse>> GetAllRequestsAsync()
        {
            var requestResponses = _dbContext.WithdrawalRequests
                .AsQueryable()
                .AsNoTracking()
                .Include(x => x.User).
                Select(x => _mapper.Map<WithdrawalRequestResponse>(x))
                .ToList();

            return requestResponses;
        }

        public async Task<Guid?> ApproveRequest(Guid requestId)
        {

            var request = _dbContext.WithdrawalRequests.Include(x => x.User).FirstOrDefault(x => x.Id == requestId);

            if (request is null)
            {
                throw new Exception($"Request {requestId} does not exist");
            }

            if (request.Status == WithdrawalRequestStatus.Approved)
            {
                throw new Exception($"Request {requestId} already approved");
            }

            if (request.User.Coin < request.WithdrawalAmount)
            {
                throw new Exception($"User {request.User.Id} doesn't have enough coin");
            }

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _balanceService.UseBalanceAsync(new UseBalanceRequest
                    {
                        Amount = 0 - request.WithdrawalAmount,
                        UserId = request.User.Id,
                        Description = $"Withdrawal {0 - request.WithdrawalAmount}",
                    }, _dbContext);

                    var logId = await _balanceService.AddUserTransactionAsync(
                        new AddUserTransactionRequest
                        {
                            Amount = 0 - request.WithdrawalAmount,
                            FromUserId = request.User.Id,
                            Type = TransactionType.WITHDRAW,
                            Status = TransactionStatus.SUCCESS,
                            DonationType = DonateType.None,
                            Description = $"Withdrawal {0 - request.WithdrawalAmount}",
                        }, _dbContext);

                    _dbContext.WithdrawalRequests.Update(request);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var eventBus = _serviceProvider.GetRequiredService<IEventBus>();

                    var actorInfo = await GetSystemActorInForAsync();

                    eventBus.Publish(new NotificationIntegrationEvent
                        (
                         userIds: new List<Guid> { request.User.Id },
                         actor: actorInfo,
                         referenceId: request.User.Id,
                         notificationType: NotificationConstant.ApproveRequestCreateWithdrawal,
                         donateAmount: request.WithdrawalAmount,
                         referenceUrl: null,
                         type: NotificationConstant.System
                         ));

                    return logId;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"WithdrawalRequest.ApproveRequest: Error={ex.Message}");

                    if (transaction.GetDbTransaction() is not null)
                        await transaction.RollbackAsync();

                    return null;
                }
            }
        }

        private void ValidateUser(Guid userId, out User user)
        {
            user = _dbContext.Users.FirstOrDefault(x => x.Id == userId);

            if (user is null)
            {
                throw new Exception($"User with Id {userId} does not exist");
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
    }
}
