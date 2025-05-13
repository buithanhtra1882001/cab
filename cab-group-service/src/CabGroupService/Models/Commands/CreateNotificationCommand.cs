using CabGroupService.IntegrationEvents.Events;
using MediatR;

namespace CabGroupService.Models.Commands
{
    public class CreateNotificationCommand : INotification
    {
        public List<Guid> UserIds { get; set; }
        public UserInfo Actor { get; set; }
        public Guid ReferenceId { get; set; }
        public string NotificationType { get; set; }
        public decimal? DonateAmount { get; set; }
    }
}
