using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;

namespace CabPostService.Models.Queries
{
    public class GetPostByUserIdQuery : IQuery<PagingResponse<GetAllPostResponse>>
    {
        public Guid UserId { get; set; }
        public string? GroupId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
