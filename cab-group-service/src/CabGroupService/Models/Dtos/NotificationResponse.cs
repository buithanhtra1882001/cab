using CabGroupService.Handlers.Interfaces;
using CabGroupService.IntegrationEvents.Events;

namespace CabGroupService.Models.Dtos
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public UserInfo Actor { get; set; }
        public Guid ReferenceId { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
