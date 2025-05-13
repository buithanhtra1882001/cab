namespace CabUserService.Models.Dtos
{
    public class UserMessageResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
    }
}
