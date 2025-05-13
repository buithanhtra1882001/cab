using GatewayApi.Infrastructures.Resiliences;
using GatewayApi.Infrastructures.Swagger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CabApiGateway.Infrastructures.Startup.ServicesExtensions
{
    public static class GeneralServiceExtension
    {
        public static void AddGeneralConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration);
            services.AddHttpClient();

            services.AddTransient<IHttpClient, HttpClient>();
            services.AddSwaggerProxy();

            services.AddCors(options =>
            {
                options.AddPolicy(configuration.GetValue<string>("CorsPolicyName"),
                    builder => builder.SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            services.AddHealthChecks();
        }
    }
}