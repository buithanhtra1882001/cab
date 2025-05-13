using CabPostService.Handlers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Commands
{
    public class DeletePostCommand : ICommand<bool>
    {
        [Required]
        public string PostId { get; set; }

        public Guid UserId { get; set; }

        public bool IsSoftDelete { get; set; }
    }
}