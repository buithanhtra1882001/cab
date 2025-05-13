using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Commands
{
    public class ReportCommand : ICommand<Guid>
    {
        [Required]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PostId { get; set; }
        public ReportReason Reason { get; set; }
        public string Description { get; set; }
    }
}
