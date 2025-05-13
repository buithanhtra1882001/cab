using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Queries
{
    public class GetCommentQuery : IQuery<PagingResponse<UserCommentResponse>>
    {
        [JsonIgnore]
        public Guid UserId { get; set; }
        [Required]
        public string PostId { get; set; }
        public string PagingState { get; set; } = string.Empty;
        public int PageSize { get; set; } = 10;
        public int PageNumber { get; set; } = 1;
    }
}
