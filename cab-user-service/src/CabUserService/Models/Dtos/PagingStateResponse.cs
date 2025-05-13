namespace CabUserService.Models.Dtos
{
    public class PagingStateResponse<T> : PagingResponse<T> where T : class
    {        
        public string PagingStateFirst { get; set; }
        public string PagingStateLast { get; set; }
    }
}
