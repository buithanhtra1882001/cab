using CabUserService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Entities
{
    public class UserFollowHistory : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid FollowerId { get; set; }
    }
}