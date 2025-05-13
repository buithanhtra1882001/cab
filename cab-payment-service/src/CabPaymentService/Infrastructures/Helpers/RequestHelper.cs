namespace CabPaymentService.Infrastructures.Helpers
{
    public static class RequestHelper
    {
        public static string GetIpAddress(this HttpRequest request)
        {
            string ipAddress;
            try
            {
                ipAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
                ipAddress = "Invalid IP:" + ex.Message;
            }

            return ipAddress;
        }
    }
}
