using CAB.BuildingBlocks.EventBus.Events;

namespace CabPostService.IntegrationEvents.Events
{
    public record UserProfileUpdateAvatarIntegrationEvent : IntegrationEvent
    {
        public Guid UserId { get; private set; }
        public string Avatar { get; private set; }

        public UserProfileUpdateAvatarIntegrationEvent(Guid userId, string avatar)
        {
            UserId = userId;
            Avatar = avatar;
        }
    }
}
