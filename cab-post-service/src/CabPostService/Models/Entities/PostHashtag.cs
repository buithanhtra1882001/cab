using CabPostService.Models.Entities.Base;

namespace CabPostService.Models.Entities
{
    public class PostHashtag : BaseEntity
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActived { get; set; }
        public double Point { get; set; }
    }
}
