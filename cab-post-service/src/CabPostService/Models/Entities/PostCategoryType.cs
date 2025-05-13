using CabPostService.Models.Entities.Base;

namespace CabPostService.Models.Entities
{
    public class PostCategoryType : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid PostCategoryId { get; set; }
        public int Type { get; set; }
    }
}
