using CabPostService.Handlers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Commands
{
    public class EditPostCommand : ICommand<bool>
    {
        [Required]
        public string PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string[] ImageUrls { get; set; }
        public string[] VideoUrls { get; set; }
    }
}
