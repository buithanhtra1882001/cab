using CabPostService.Handlers.Interfaces;

namespace CabPostService.Models.Commands
{
    public class UpdateUserBehaviorHiddenCommand : ICommand<bool>
    {
        public Guid UserId { get; set; }
        public string PostId { get; set; }
        public bool HideAll { get; set; } = false;
    }
}
