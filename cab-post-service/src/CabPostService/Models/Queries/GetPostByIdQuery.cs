using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Queries
{
    public class GetPostByIdQuery : IQuery<GetAllPostResponse?>
    {
        [Required]
        public string PostId { get; set; }
    }
}

