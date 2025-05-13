using CAB.BuildingBlocks.EventBus.Events;

namespace CabUserService.IntegrationEvents.Events
{
    public record UserDeletedIntegrationEvent : IntegrationEvent
    {
        public string Email { get; private set; }

        public UserDeletedIntegrationEvent(string email)
        {
            Email = email;
        }
    }
}
