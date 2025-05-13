using CabPostService.Handlers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Commands
{
    public class ReplyCommand : ICommand<Guid>
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ReplyToCommentId { get; set; }

        public string Content { get; set; }
    }
}
