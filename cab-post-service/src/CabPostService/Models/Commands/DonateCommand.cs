using CabPostService.Handlers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Commands
{
    public class DonateCommand : ICommand<Guid>
    {
        [Required]
        public Guid Auid { get; set; }
        public string PostId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Value { get; set; }
    }
}
