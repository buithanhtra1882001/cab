namespace CabUserService.Models.Dtos
{
    public class MessagesResponse
    {
        public Guid SenderUserId { get; set; }
        public Guid RecipientUserId { get; set; }
        public string SenderName { get; set; }
        public string RecipientName { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } 
    }
}
