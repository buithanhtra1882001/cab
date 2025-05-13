using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Dtos
{
    public class UserDonationRequest
    {
        [Required]
        public Guid ToUserId { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public string Message { get; set; }
    }
}