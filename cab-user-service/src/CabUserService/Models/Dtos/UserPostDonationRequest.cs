using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Dtos
{
    public class UserPostDonationRequest : UserDonationRequest
    {
        public Guid? PostId { get; set; }
    }
}
