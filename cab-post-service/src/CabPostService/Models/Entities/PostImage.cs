using CabPostService.Models.Entities.Base;

namespace CabPostService.Models.Entities
{
    public class PostImage : BaseEntity
    {
        public Guid Id { get; set; }
        public string PostId { get; set; }
        public Guid ImageId { get; set; }
        public string Url { get; set; }
        public bool IsViolence { get; set; }
    }
}