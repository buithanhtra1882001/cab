namespace CabUserService.Models.Dtos
{
    public class UserOnlineResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public bool IsOnline { get; set; }
        public DateTime LastTime { get; set; }
    }
}
