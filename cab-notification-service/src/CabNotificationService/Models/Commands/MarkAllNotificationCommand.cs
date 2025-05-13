using CabNotificationService.Handlers.Interfaces;
using Newtonsoft.Json;

namespace CabNotificationService.Models.Commands
{
    public class MarkAllNotificationCommand : ICommand<bool>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public bool IsRead { get; set; }
    }
}
