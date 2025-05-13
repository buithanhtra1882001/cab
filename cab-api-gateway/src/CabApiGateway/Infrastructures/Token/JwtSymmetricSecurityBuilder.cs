using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CabApiGateway.Infrastructures.Token
{
    public static class JwtSymmetricSecurityBuilder
    {
        public static string GetConfigKeyAsString(IConfiguration configuration)
        {
            return configuration.GetValue<string>("Authentication:JwtSigninKey");
        }

        public static byte[] GetConfigKeyAsBytes(IConfiguration configuration)
        {
            var issuerSigningKey = GetConfigKeyAsString(configuration);
            var data = Encoding.UTF8.GetBytes(issuerSigningKey);
            return data;
        }

        public static SymmetricSecurityKey BuildKey(IConfiguration configuration)
        {
            var data = GetConfigKeyAsBytes(configuration);
            var result = new SymmetricSecurityKey(data);
            return result;
        }
    }
}