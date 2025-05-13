namespace WCABNetwork.Cab.IdentityService.Models.Dtos.Requests
{
    public class ResetPasswordRequest
    {
        //[Required(AllowEmptyStrings = false)]
        public string Email { get; set; }
    }
}
