using CabPostService.Constants;

namespace CabPostService.Models.Dtos
{
    public class PagingOrderedBase
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int Skip => (PageNumber - 1) * PageSize;

        public string PagingQueryString => $" ORDER BY \"CreatedAt\" DESC OFFSET {Skip} LIMIT {PageSize}";
        public string? GroupId { get; set; }
        public GroupPostStatus GroupPostStatus { get; set; }
    }
}
