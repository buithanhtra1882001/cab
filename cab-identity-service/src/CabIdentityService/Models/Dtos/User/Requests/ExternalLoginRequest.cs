namespace WCABNetwork.Cab.IdentityService.Models.Dtos.Requests
{
    public class ExternalLoginRequest
    {
        public string Provider { get; set; }
        public string AccessToken { get; set; }
        public string PassCode { get; set; } = string.Empty;
    }
}
