using System.ComponentModel.DataAnnotations;

namespace WCABNetwork.Cab.IdentityService.Models.Dtos.Requests
{
    public class SetNewPasswordRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Token { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
