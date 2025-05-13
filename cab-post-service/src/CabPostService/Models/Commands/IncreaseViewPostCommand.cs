using CabPostService.Handlers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Commands
{
    public class IncreaseViewPostCommand : ICommand<long>
    {
        [Required]
        public string PostId { get; set; }
    }
}
