using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Queries
{
    public class GetPostBySlugQuery : IQuery<IList<UserPostResponse>>
    {
        [Required]
        public string Slug { get; set; }
    }
}

