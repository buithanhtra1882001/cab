using CabMediaService.Constants;

namespace CabMediaService.Models.Dtos
{
    public class UserServiceResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public UserData Data { get; set; }
    }
    public class UserData
    {
        public string Phone { get; set; }
        public string IdentityCardNumber { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Dob { get; set; }
        public string City { get; set; }
        public string Avatar { get; set; }
        public string Sex { get; set; }
        public bool IsUpdateProfile { get; set; }
        public string Description { get; set; }
        public decimal Balance { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalFriend { get; set; }
        public bool IsFriend { get; set; }
        public bool IsFollow { get; set; }
        public bool IsFriendRequest { get; set; }
        public string CoverImage { get; set; }
        public UserType UserType { get; set; }
    }
}
