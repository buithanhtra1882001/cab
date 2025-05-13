namespace CabPostService.Models.Dtos
{
    public class PagingResponse<T> where T : class
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string PagingState { get; set; }
        public long Total { get; set; } 
        public bool HasMore { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}
