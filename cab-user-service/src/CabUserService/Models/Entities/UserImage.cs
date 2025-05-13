using CabUserService.Models.Entities.Base;

namespace CabUserService.Models.Entities
{
    public class UserImage : BaseEntity
    {
        public Guid UserId { get; set; }
        public string Url { get; set; }
        public double Size { get; set; }
    }
}
