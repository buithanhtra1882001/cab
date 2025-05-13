using Newtonsoft.Json;

namespace CabUserService.Models.Dtos
{
    public class UserFriendOnlineRequest:PagingRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
