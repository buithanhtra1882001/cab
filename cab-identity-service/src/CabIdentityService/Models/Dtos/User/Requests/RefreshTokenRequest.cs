using System.ComponentModel.DataAnnotations;

namespace WCABNetwork.Cab.IdentityService.Models.Dtos.Requests
{
    public class RefreshTokenRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string RefreshToken { get; set; }
        public string FingerprintHash { get; set; }
    }
}
