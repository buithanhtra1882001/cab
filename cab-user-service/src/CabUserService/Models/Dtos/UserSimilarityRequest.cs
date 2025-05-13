using CabUserService.Constants;

namespace CabUserService.Models.Dtos
{
    public class UserSimilarityRequest
    {
        public Guid UserId { get; set; }
        public string Sex { get; set; }
        public string Dob { get; set; }

        public UserType UserType { get; set; }

    }
}
