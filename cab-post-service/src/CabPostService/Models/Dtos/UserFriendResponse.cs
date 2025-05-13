namespace CabPostService.Models.Dtos
{
    public class UserFriendResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
    }
    public class ApiResponse<T>
    {
        public int Total { get; set; }
        public bool HasNext { get; set; }
        public List<T> Data { get; set; }
    }
}
