using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Events;

namespace WCABNetwork.Cab.IdentityService.IntegrationEvents.Events
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
