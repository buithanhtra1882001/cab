using System.ComponentModel.DataAnnotations;

namespace WCABNetwork.Cab.IdentityService.Models.Dtos.Requests
{
    public class RegisterRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string FullName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string UserName { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
