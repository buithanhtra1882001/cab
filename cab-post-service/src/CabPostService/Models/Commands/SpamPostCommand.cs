using System.ComponentModel.DataAnnotations;
using CabPostService.Handlers.Interfaces;

namespace CabPostService.Models.Commands
{
    public class SpamPostCommand : ICommand<bool>
    {
        [Required]
        public string PostId { get; set; }

        public Guid UserId { get; set; }
    }
}
