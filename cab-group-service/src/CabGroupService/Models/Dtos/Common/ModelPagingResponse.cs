namespace CabGroupService.Models.Dtos
{
    public class ModelPagingResponse<T>
    {
        public IEnumerable<T>? Data { get; set; }
        public long Total { get; set; }
    }
}
