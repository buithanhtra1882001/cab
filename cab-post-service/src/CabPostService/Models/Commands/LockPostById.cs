using CabPostService.Handlers.Interfaces;

namespace CabPostService.Models.Commands
{
    public class LockPostById : ICommand<bool>
    {
        public string PostId { get; set; }
    }
}
