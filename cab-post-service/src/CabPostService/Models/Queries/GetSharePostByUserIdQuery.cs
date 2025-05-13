using System.ComponentModel.DataAnnotations;
using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;

namespace CabPostService.Models.Queries
{
  public class GetSharePostByUserIdQuery : IQuery<PagingResponse<SharePostResponse>>
  {
    [Required]
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
  }
}
