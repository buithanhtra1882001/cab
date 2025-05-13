using CAB.BuildingBlocks.EventBus.Events;

namespace CabPostService.IntegrationEvents.Events
{
    public record UserProfileCreatedIntegrationEvent : IntegrationEvent
    {
        public Guid UserId { get; private set; }
        public string Username { get; private set; }
        public string Fullname { get; private set; }
        public string Avatar { get; private set; }

        public UserProfileCreatedIntegrationEvent(
            Guid userId,
            string userName,
            string fullName,
            string avatar)
        {
            UserId = userId;
            Username = userName;
            Fullname = fullName;
            Avatar = avatar;
        }
    }
}
