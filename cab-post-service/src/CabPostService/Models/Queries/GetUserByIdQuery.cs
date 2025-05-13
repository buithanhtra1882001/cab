using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Queries
{
    public class GetUserByIdQuery : IQuery<UserResponse?>
    {
        [Required]
        public Guid UserId { get; set; }
    }
}
