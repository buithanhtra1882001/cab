using CAB.BuildingBlocks.EventBus.Abstractions;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.IntegrationEvents.Events;
using CabUserService.Models.Entities;

namespace CabUserService.IntegrationEvents.EventHandlers
{
    public class UserRegisterIntegrationEventHandler : IIntegrationEventHandler<UserRegisterIntegrationEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly ILogger<UserRegisterIntegrationEventHandler> _logger;

        public UserRegisterIntegrationEventHandler(IUserRepository userRepository, IUserDetailRepository userDetailRepository, ILogger<UserRegisterIntegrationEventHandler> logger)
        {
            _userRepository = userRepository;
            _userDetailRepository = userDetailRepository;
            _logger = logger;
        }
        public async Task Handle(UserRegisterIntegrationEvent @event)
        {
            var existsUser = await _userRepository.GetByIdAsync(Guid.Parse(@event.UserId));
            if (existsUser != null)
            {
                existsUser.IsVerifyEmail = @event.IsVerifyEmail;
                await _userRepository.UpdateAsync(existsUser);
                return;
            }
            var newUser = new User
            {
                FullName = @event.FullName,
                Email = @event.Email,
                Id = Guid.Parse(@event.UserId),
                UserName = @event.UserName,
                IsVerifyEmail = @event.IsVerifyEmail
            };
            var newUserDetail = new UserDetail { UserId = Guid.Parse(@event.UserId), UserDetailId = Guid.NewGuid() };

            await _userRepository.CreateAsync(newUser);
            await _userDetailRepository.CreateAsync(newUserDetail);
            _logger.LogInformation("Created new user {@user}", newUser);
            _logger.LogInformation("Created new user {@userdetail}", newUserDetail);
        }
    }
}
