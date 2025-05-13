using CAB.BuildingBlocks.EventBus.Events;

namespace CabUserService.IntegrationEvents.Events
{
    public record NotificationIntegrationEvent : IntegrationEvent
    {
        public List<Guid> UserIds { get; set; }
        public UserInfo Actor { get; set; }
        public Guid ReferenceId { get; set; }
        public string NotificationType { get; set; }
        public double? DonateAmount { get; set; }
        public string ReferenceUrl { get; set; } = null;
        public string Type { get; set; } = null;

        public NotificationIntegrationEvent
            (List<Guid> userIds, UserInfo actor, Guid referenceId, string notificationType, double? donateAmount, string referenceUrl, string type)
        {
            UserIds = userIds;
            Actor = actor;
            ReferenceId = referenceId;
            NotificationType = notificationType;
            DonateAmount = donateAmount;
            ReferenceUrl = referenceUrl;
            Type = type;
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