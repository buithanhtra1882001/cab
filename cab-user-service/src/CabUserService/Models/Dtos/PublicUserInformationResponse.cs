using CabUserService.Constants;

namespace CabUserService.Models.Dtos
{
    public class PublicUserInformationResponse
    {
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
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalFriend { get; set; }
        public bool IsFriend { get; set; }
        public bool IsFollow { get; set; }
        public bool IsFriendRequest { get; set; }
        public bool IsVerifyEmail { get; set; }
        public bool IsCreateRequestReciveDonate { get; set; }
        public string CoverImage { get; set; }
        public bool CanReceiveDonation { get; set; }
        public double Coin { get; set; }

        public List<UserCategories>? CategoryFavorites { get; set; }

        public UserType UserType { get; set; }
    }

    public class UserCategories
    {
        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }
    }
}
