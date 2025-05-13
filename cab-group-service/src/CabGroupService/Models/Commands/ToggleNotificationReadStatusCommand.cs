using CabGroupService.Handlers.Interfaces;
using Newtonsoft.Json;

namespace CabGroupService.Models.Commands
{
    public class ToggleNotificationReadStatusCommand : ICommand<bool>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
    }
}
