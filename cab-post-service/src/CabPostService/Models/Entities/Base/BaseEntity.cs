namespace CabPostService.Models.Entities.Base
{
    public class BaseEntity
    {
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}