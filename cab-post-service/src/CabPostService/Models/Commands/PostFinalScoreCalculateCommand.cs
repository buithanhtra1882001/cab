using CabPostService.Handlers.Interfaces;

namespace CabPostService.Models.Commands
{
    public class PostFinalScoreCalculateCommand : ICommand<decimal>
    {
        public Entities.Post Post { get; set; }

        public Guid UserId { get; set; }
    }
}
