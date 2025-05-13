using Newtonsoft.Json;

namespace CabUserService.Models.Dtos
{
    public class GetUserFriendsRequest : PagingRequest
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
    }
}
