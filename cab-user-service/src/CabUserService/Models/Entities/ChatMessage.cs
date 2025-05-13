using CabUserService.Models.Entities.Base;

namespace CabUserService.Models.Entities
{
    public class ChatMessage : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid SenderUserId { get; set; }
        public Guid RecipientUserId { get; set; }
        public string Content { get; set; }
    }
}
