namespace CabUserService.Models.Dtos
{
    public class GetAllUserRequest : PagingRequest
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public short? Status { get; set; }
    }
}
