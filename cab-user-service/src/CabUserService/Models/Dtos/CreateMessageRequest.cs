using Newtonsoft.Json;

namespace CabUserService.Models.Dtos
{
    public class CreateMessageRequest
    {
        [JsonIgnore]       
        public Guid SenderUserId { get; set; }
        public Guid RecipientUserId { get; set; }
        public string Content { get; set; }
    }
}
