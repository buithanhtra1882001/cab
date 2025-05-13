using Newtonsoft.Json;

namespace CabUserService.Models.Dtos
{
    public class GetContentMessageRequest : PagingRequest
    {
        [JsonIgnore]
        public Guid CurrentUserId { get; set; }
        public Guid FriendUserId { get; set; }
        public string PagingStateFirst { get; set; } = string.Empty;
        public string PagingStateLast { get; set; } = string.Empty;
    }
}
