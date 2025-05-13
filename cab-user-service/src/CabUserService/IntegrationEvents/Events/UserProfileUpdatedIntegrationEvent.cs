using CAB.BuildingBlocks.EventBus.Events;

namespace CabUserService.IntegrationEvents.Events
{
    public record UserProfileUpdatedIntegrationEvent : IntegrationEvent
    {
        public Guid UserId { get; private set; }
        public string Avatar {  get; private set; }
        public string Username { get; private set; }
        public string Fullname { get; private set; }

        public UserProfileUpdatedIntegrationEvent(Guid userId, string avatar, string userName, string fullName)
        {
            UserId = userId;
            Avatar = avatar;
            Username = userName;
            Fullname = fullName;
        }
    }
}
