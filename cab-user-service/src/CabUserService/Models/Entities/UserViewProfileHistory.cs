using CabUserService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Entities
{
    public class UserViewProfileHistory : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public Guid ViewerId { get; set; }
    }
}