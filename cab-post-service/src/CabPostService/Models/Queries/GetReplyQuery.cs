using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Queries
{
    public class GetReplyQuery : IQuery<PagingResponse<UserReplyResponse>>
    {
        [Required]
        public Guid CommentId { get; set; }
        public string PagingState { get; set; } = string.Empty;
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
    }
}
