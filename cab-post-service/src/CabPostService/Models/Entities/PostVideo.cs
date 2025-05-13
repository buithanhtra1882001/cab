using CabPostService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Entities
{
    public class PostVideo : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public string PostId { get; set; }
        public Guid UserId { get; set; }
        public string MediaVideoId { get; set; }
        public string VideoUrl { get; set; }
        public string Description { get; set; }
        public double LengthVideo { get; set; }
        public double AvgViewLength { get; set; }
        public int ViewCount { get; set; }
        public bool IsViolence { get; set; }
    }
}
