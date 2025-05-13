using CabUserService.Models.Entities.Base;

namespace CabUserService.Models.Entities
{
    public class ChatUserConnection:BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ConnectionId { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastTime { get; set; } = DateTime.UtcNow;
    }
}
