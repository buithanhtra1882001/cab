using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Queries
{
    public class GetPostCategoryBySlugQuery : IQuery<UserPostCategoryResponse>
    {
        [Required]
        public string Slug { get; set; }
    }
}
