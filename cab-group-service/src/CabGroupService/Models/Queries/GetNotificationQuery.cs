using CabGroupService.Handlers.Interfaces;
using CabGroupService.Models.Dtos;
using Newtonsoft.Json;

namespace CabGroupService.Models.Queries
{
    public class GetNotificationQuery : PagingRequest, IQuery<PagingResponse<NotificationResponse>>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string PagingState { get; set; } = string.Empty;
    }
}
