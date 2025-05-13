using CabNotificationService.Models.Entities.Base;

namespace CabNotificationService.Models.Entities
{
    public class Notification : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ActorId { get; set; }
        public Guid ReferenceId { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }
        public bool IsRead { get; set; }
        public string? ReferenceUrl { get; set; }
        public string? Type { get; set; }
    }
}
