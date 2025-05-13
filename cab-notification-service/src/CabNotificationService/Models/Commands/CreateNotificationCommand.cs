using CabNotificationService.IntegrationEvents.Events;
using MediatR;

namespace CabNotificationService.Models.Commands
{
    public class CreateNotificationCommand : INotification
    {
        public List<Guid> UserIds { get; set; }
        public UserInfo Actor { get; set; }
        public Guid ReferenceId { get; set; }
        public string NotificationType { get; set; }
        public double? DonateAmount { get; set; }        
        public string? ReferenceUrl { get; set; }
        public string? Type { get; set; }
    }
}
