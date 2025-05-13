using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Queries
{
    public class GetPostCategoryByTypeQuery : IQuery<IList<UserPostCategoryResponse>>
    {
        [Required]
        public int Type { get; set; }
    }
}
