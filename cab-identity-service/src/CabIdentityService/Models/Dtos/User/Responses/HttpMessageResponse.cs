namespace WCABNetwork.Cab.IdentityService.Models.Dtos
{
    public class HttpMessageResponse
    {
        public HttpMessageResponse()
        {
        }

        public HttpMessageResponse(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}
