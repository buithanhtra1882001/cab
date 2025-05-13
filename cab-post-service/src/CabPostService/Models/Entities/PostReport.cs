using CabPostService.Constants;
using CabPostService.Models.Entities.Base;

namespace CabPostService.Models.Entities
{
    public class PostReport : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PostId { get; set; }
        public ReportReason Reason { get; set; }
        public string Description { get; set; }
    }
}
