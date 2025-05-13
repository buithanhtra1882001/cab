using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Dtos
{
    public class FollowCategoryRequest
    {
        [Required(AllowEmptyStrings = false)]
        public Guid UserId { get; set; }
        [Required(AllowEmptyStrings = false)]
        public Guid CategoryId { get; set; }
    }
}
