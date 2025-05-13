using AutoMapper;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CabNotificationService.Constants;
using CabUserService.Infrastructures.DbContexts;
using CabUserService.Infrastructures.Repositories.Interfaces;
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
    public class DonateReceiverRequestService : BaseService<DonateReceiverRequestService>, IDonateReceiverRequestService
    {
        private IServiceProvider _serviceProvider;

        private readonly IMapper _mapper;

        private readonly PostgresDbContext _dbContext;

        private readonly IDonateReceiverRequestRepository _repository;

        public DonateReceiverRequestService(
            ILogger<DonateReceiverRequestService> logger,
            IServiceProvider serviceProvider,
            IMapper mapper,
            PostgresDbContext postgresDbContext,
            IDonateReceiverRequestRepository repository)
        : base(logger)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _dbContext = postgresDbContext;
            _repository = repository;
        }

        public async Task<DonateReceiverRequestResponse> CreateRequestAsync(Guid userId, DonateReceiverRequestDto requestDto)
        {
            ValidateUser(userId, out var user);

            var requestAlreadyExists = _dbContext.DonateReceiverRequests.FirstOrDefault(x => x.User == user) != null;
            if (requestAlreadyExists)
            {
                throw new Exception($"Request of user {userId} already exists");
            }

            if (!user.IsVerifyEmail)
            {
                throw new Exception($"User {userId} is not allowed to create receive donation request, email verification needed");
            }


            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var newRequest = _mapper.Map<DonateReceiverRequest>(requestDto);

                    newRequest.Id = new Guid();
                    newRequest.User = user;
                    newRequest.Status = DonateReceiverRequestStatus.Pending;
                    newRequest.UpdatedAt = DateTime.Now;
                    newRequest.CreatedAt = DateTime.Now;

                    _dbContext.DonateReceiverRequests.Add(newRequest);

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var eventBus = _serviceProvider.GetRequiredService<IEventBus>();
                    var actorInfo = await GetSystemActorInForAsync();

                    eventBus.Publish(new NotificationIntegrationEvent
                        (
                         userIds: new List<Guid> { user.Id },
                         actor: actorInfo,
                         referenceId: user.Id,
                         notificationType: NotificationConstant.CreateRequestReceiveDonation,
                         donateAmount: null,
                         referenceUrl: null,
                         type: NotificationConstant.System
                         ));

                    var response = _mapper.Map<DonateReceiverRequestResponse>(newRequest);
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"DonateDonateReceiverRequestService.CreateRequestAsync: Error={ex.Message}");

                    if (transaction.GetDbTransaction() is not null)
                        await transaction.RollbackAsync();

                    return null;
                }
            }
        }

        public async Task<DonateReceiverRequestResponse> GetRequestAsync(Guid auid)
        {
            var request = _dbContext.DonateReceiverRequests.AsNoTracking().Include(x => x.User).FirstOrDefault(x => x.User.Id == auid);
            return request is null ? null : _mapper.Map<DonateReceiverRequestResponse>(request);
        }

        public async Task DeleteRequestAsync(Guid userId, Guid id)
        {
            ValidateUser(userId, out _);
            var request = _dbContext.DonateReceiverRequests.FirstOrDefault(x => x.Id == id);

            if (request is null)
            {
                throw new Exception($"Request with Id {id} does not exist");
            }

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    _dbContext.DonateReceiverRequests.Remove(request);
                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"DonateDonateReceiverRequestService.DeleteRequestAsync: Error={ex.Message}");

                    if (transaction.GetDbTransaction() is not null)
                        await transaction.RollbackAsync();

                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<List<DonateReceiverRequestResponse>> GetAllRequestsAsync()
        {
            var requestResponses = _dbContext.DonateReceiverRequests
                .AsQueryable()
                .AsNoTracking()
                .Include(x => x.User).
                Select(x => _mapper.Map<DonateReceiverRequestResponse>(x))
                .ToList();

            return requestResponses;
        }

        public async Task<DonateReceiverRequestResponse> UpdateRequestAsync(Guid userId, DonateReceiverRequestDto requestDto)
        {
            ValidateUser(userId, out var user);

            var request = _dbContext.DonateReceiverRequests.FirstOrDefault(x => x.User.Id == userId);

            if (request is null)
            {
                throw new Exception($"Request of user Id {userId} does not exist");
            }

            if (request.Status == DonateReceiverRequestStatus.Approved)
            {
                throw new Exception($"Request of user Id {userId} is already approved");
            }

            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    request.ReferenceLinks = requestDto.ReferenceLinks;
                    request.BankName = requestDto.BankName;
                    request.BankAccount = requestDto.BankAccount;
                    request.NationalId = requestDto.NationalId;
                    request.UpdatedAt = DateTime.UtcNow;
                    _dbContext.DonateReceiverRequests.Update(request);

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var response = _mapper.Map<DonateReceiverRequestResponse>(request);
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"DonateDonateReceiverRequestService.UpdateRequestAsync: Error={ex.Message}");

                    if (transaction.GetDbTransaction() is not null)
                        await transaction.RollbackAsync();

                    return null;
                }
            }
        }

        public async Task ApproveRequest(Guid auid, Guid userToBeApproveId)
        {
            ValidateUser(userToBeApproveId, out var user);

            var request = _dbContext.DonateReceiverRequests.FirstOrDefault(x => x.User.Id == userToBeApproveId);

            if (request is null)
            {
                throw new Exception($"Request of user {userToBeApproveId} does not exist");
            }

            if (request.Status == DonateReceiverRequestStatus.Approved && user.CanReceiveDonation)
            {
                throw new Exception($"User {userToBeApproveId} already can receive donation");
            }


            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    request.Status = DonateReceiverRequestStatus.Approved;
                    user.CanReceiveDonation = true;
                    request.UpdatedAt = user.UpdatedAt = DateTime.UtcNow;

                    _dbContext.DonateReceiverRequests.Update(request);
                    _dbContext.Users.Update(user);

                    await _dbContext.SaveChangesAsync();
                    await transaction.CommitAsync();

                    var eventBus = _serviceProvider.GetRequiredService<IEventBus>();
                    var actorInfo = await GetSystemActorInForAsync();

                    eventBus.Publish(new NotificationIntegrationEvent
                        (
                         userIds: new List<Guid> { user.Id },
                         actor: actorInfo,
                         referenceId: user.Id,
                         notificationType: NotificationConstant.ApproveRequestCreateWithdrawal,
                         donateAmount: null,
                         referenceUrl: null,
                         type: NotificationConstant.System
                         ));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"DonateDonateReceiverRequestService.ApproveRequest: Error={ex.Message}");

                    if (transaction.GetDbTransaction() is not null)
                        await transaction.RollbackAsync();

                    throw new Exception(ex.Message);
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
