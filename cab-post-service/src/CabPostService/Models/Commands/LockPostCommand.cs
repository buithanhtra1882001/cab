using CabPostService.Handlers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Commands
{
    public class LockPostCommand : ICommand<bool>
    {
        [Required]
        public string PostId { get; set; }
    }
}
