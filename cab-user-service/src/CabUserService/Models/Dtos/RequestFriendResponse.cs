namespace CabUserService.Models.Dtos
{
    public class RequestFriendResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
    }
}
