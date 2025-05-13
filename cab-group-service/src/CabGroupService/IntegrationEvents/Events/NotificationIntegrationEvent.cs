using CAB.BuildingBlocks.EventBus.Events;

namespace CabGroupService.IntegrationEvents.Events
{
    public record NotificationIntegrationEvent : IntegrationEvent
    {
        public List<Guid> UserIds { get; set; }
        public UserInfo Actor { get; set; }
        public Guid ReferenceId { get; set; }
        public string NotificationType { get; set; }
        public int? DonateAmount { get; set; }

        public NotificationIntegrationEvent(List<Guid> userIds, UserInfo actor, Guid referenceId, string notificationType, int? donateAmount)
        {
            UserIds = userIds;
            Actor = actor;
            ReferenceId = referenceId;
            NotificationType = notificationType;
            DonateAmount = donateAmount;
        }
    }

    public record UserInfo
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }

        public UserInfo(Guid userId, string fullName, string avatar)
        {
            UserId = userId;
            FullName = fullName;
            Avatar = avatar;
        }
    }

}