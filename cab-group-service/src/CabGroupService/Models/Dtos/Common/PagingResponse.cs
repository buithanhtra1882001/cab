namespace CabGroupService.Models.Dtos
{
    public class PagingResponse<T> where T : class
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public long Total { get; set; }
        public List<T> Elements { get; set; } = new List<T>();
    }
}
