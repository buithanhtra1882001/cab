using CabGroupService.Models.Entities.Base;

namespace CabGroupService.Models.Entities
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
    }
}
