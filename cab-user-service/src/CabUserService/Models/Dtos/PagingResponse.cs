namespace CabUserService.Models.Dtos
{
    public class PagingResponse<T> where T : class
    {
        public long Total { get; set; }
        public bool HasNext { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}
