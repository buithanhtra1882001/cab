using CabPostService.Handlers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Commands
{
    public class DeletePostCategoryCommand : ICommand<bool>
    {
        [Required]
        public Guid PostCategoryId { get; set; }

        public bool IsSoftDelete { get; set; }
    }
}