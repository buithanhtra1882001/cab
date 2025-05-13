using CAB.BuildingBlocks.EventBus.Events;

namespace CabUserService.IntegrationEvents.Events
{
    public record UserCreatedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; private set; }
        public string Email { get; private set; }
        public string FullName { get; private set; }
        public UserCreatedIntegrationEvent(string userId, string email, string fullName)
        {
            UserId = userId;
            Email = email;
            FullName = fullName;
        }
    }
}
