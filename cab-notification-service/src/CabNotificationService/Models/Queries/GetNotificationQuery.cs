using CabNotificationService.Handlers.Interfaces;
using CabNotificationService.Models.Dtos;
using Newtonsoft.Json;

namespace CabNotificationService.Models.Queries
{
    public class GetNotificationQuery : PagingRequest, IQuery<PagingResponse<NotificationResponse>>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public string PagingState { get; set; } = string.Empty;
        public string? Type { get; set; }
    }
}
