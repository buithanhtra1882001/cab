using CabUserService.Models.Entities.Base;

namespace CabUserService.Models.Entities
{
    public class Category:BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public int SortOrder { get; set; }
    }
}
