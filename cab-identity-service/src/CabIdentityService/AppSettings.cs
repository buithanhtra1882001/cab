using System.Collections.Generic;

namespace WCABNetwork.Cab.IdentityService
{
    public class AuthenticationSetting
    {
        public int AccessTokenExpiredTimeInSecond { get; set; }
        public int RefreshTokenExpiredTimeInSecond { get; set; }
    }

    public class Provider
    {
        public string Name { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }

    public class STMPSetting
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
    }

    public class AppSettings
    {
        public AuthenticationSetting Authentication { get; set; }
        public IEnumerable<Provider> Providers { get; set; }
        public STMPSetting STMPSetting { get; set; }
        public string APIGatewayHost { get; set; }
        public string FrontendHostURL { get; set; }
    }
}