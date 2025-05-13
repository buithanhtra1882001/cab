namespace CabPostService.Models.Dtos
{
    public class GetPostVideosOrderByCreatedWithPaging : PagingFilterBase
    {
        public string? GroupId { get; set; }
    }
}
