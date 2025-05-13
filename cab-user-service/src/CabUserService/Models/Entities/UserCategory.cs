using CabUserService.Models.Entities.Base;

namespace CabUserService.Models.Entities
{
    public class UserCategory:BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
