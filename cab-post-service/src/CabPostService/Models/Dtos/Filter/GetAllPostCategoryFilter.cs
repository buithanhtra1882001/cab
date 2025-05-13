namespace CabPostService.Models.Dtos
{
    public class GetAllPostCategoryFilter
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? Status { get; set; }
    }
}
