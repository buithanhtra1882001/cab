using CabPostService.Handlers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Commands
{
    public class CommentCommand : ICommand<Guid>
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string PostId { get; set; }
        public string Content { get; set; }
    }
}
