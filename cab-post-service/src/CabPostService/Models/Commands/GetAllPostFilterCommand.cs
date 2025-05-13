using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;

namespace CabPostService.Models.Commands
{
    public class GetAllPostFilterCommand : IQuery<PagingResponse<AdminGetPostsResponse>>
    {
        public string Slug { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsSoftDeleted { get; set; }
        public int? Status { get; set; }
        public bool? IsChecked { get; set; }
    }
}
