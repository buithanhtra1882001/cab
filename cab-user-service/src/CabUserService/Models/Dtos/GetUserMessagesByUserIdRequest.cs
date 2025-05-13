using Newtonsoft.Json;

namespace CabUserService.Models.Dtos
{
    public class GetUserMessagesByUserIdRequest:PagingRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
