using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Dtos
{
    public class FollowCategoriesRequest
    {
         [Required(AllowEmptyStrings = false)]
        public Guid UserId { get; set; }
        [Required(AllowEmptyStrings = false)]
        public List<Guid> CategoryIds { get; set; }
    }
}
