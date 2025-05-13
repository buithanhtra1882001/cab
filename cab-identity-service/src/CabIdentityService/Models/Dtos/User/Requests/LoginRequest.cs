using System.ComponentModel.DataAnnotations;

namespace WCABNetwork.Cab.IdentityService.Models.Dtos.Requests
{
    public class LoginRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
        public string PassCode { get; set; } = string.Empty;
    }
}
