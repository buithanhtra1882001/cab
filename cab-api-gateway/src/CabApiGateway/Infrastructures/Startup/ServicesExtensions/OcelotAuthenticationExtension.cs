using CabApiGateway.Infrastructures.Token;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Provider.Polly;

namespace CabApiGateway.Infrastructures.Startup.ServicesExtensions
{
    public static class OcelotAuthenticationExtension
    {
        public static void AddOcelotAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var key = JwtSymmetricSecurityBuilder.BuildKey(configuration);
            var validationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                IssuerSigningKey = key
            };
            services.AddAuthentication()
                .AddJwtBearer(configuration.GetValue<string>("Authentication:Scheme"), x =>
                {
                    x.TokenValidationParameters = validationParameters;
                });

            services.AddOcelot().AddPolly();
        }
    }
}