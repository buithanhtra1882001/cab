using CAB.BuildingBlocks.EventBus.Abstractions;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.IntegrationEvents.Events;
using CabUserService.Models.Entities;

namespace CabUserService.IntegrationEvents.EventHandlers
{
    public class UserCreatedIntegrationEventHandler : IIntegrationEventHandler<UserCreatedIntegrationEvent>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserCreatedIntegrationEventHandler> _logger;

        public UserCreatedIntegrationEventHandler(IUserRepository userRepository, ILogger<UserCreatedIntegrationEventHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task Handle(UserCreatedIntegrationEvent @event)
        {
            var newUser = new User { FullName = @event.FullName, Email = @event.Email, Id = Guid.Parse(@event.UserId) };

            await _userRepository.CreateAsync(newUser);

            _logger.LogInformation("Created new user {@user}", newUser);
        }
    }
}
