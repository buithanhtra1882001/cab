using CabPostService.Models.Entities.Base;

namespace CabPostService.Models.Entities
{
    public class PostCategory : BaseEntity
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Thumbnail { get; set; }
        public int Status { get; set; }
        public decimal Score { get; set; }
        public bool IsSoftDeleted { get; set; }
        public int Position { get; set; }
    }
}
