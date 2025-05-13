using CabPostService.Handlers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Commands
{
    public class VoteDownCommentCommand : ICommand<bool>
    {
        [Required]
        public Guid CommentId { get; set; }
    }
}
