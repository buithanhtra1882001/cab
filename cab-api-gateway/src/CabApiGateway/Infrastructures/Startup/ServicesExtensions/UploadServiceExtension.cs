using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CabApiGateway.Infrastructures.Startup.ServicesExtensions
{
    public static class UploadServiceExtension
    {
        public static void AddUploadServices(this IServiceCollection services)
        {
            const int maxRequestLimit = 104857600; // 100 MB

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = maxRequestLimit;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = maxRequestLimit;
            });

            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = maxRequestLimit;
                options.MultipartBodyLengthLimit = maxRequestLimit;
                options.MultipartHeadersLengthLimit = maxRequestLimit;
            });
        }
    }
}
