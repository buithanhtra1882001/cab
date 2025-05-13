namespace CabApiGateway
{
    public class AppSettings
    {
        public SwaggerConfiguration[] GroupSwaggerConfigs { get; set; }
        public string GatewayUrl { get; set; }
    }

    public class SwaggerConfiguration
    {
        public string SwaggerName { get; set; }
        public string SwaggerEndpoint { get; set; }
        public string UpstreamTemplate { get; set; }
        public string DownstreamTemplate { get; set; }
    }
}