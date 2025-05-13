using CabUserService.Infrastructures.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Dtos
{
    public class FollowRequest
    {
        [Required(AllowEmptyStrings = false)]
        public Guid UserId { get; set; }

        [Required(AllowEmptyStrings = false)]
        public Guid FollowId { get; set; }
    }
}