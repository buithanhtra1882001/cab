using CabUserService.Controllers.Base;
using CabUserService.Infrastructures.Conventions;
using MediatR;
using Newtonsoft.Json.Converters;

namespace CabUserService.Infrastructures.Startup.ServicesExtensions
{
    public static class GeneralServiceExtension
    {
        public static void AddGeneralConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration);
            services.AddAutoMapper(typeof(CabUserService.AppSettings));

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers(options =>
                {
                    options.Conventions.Add(new ApiControllerConvention(typeof(ApiController<>)));
                })
                .AddNewtonsoftJson(options =>
                     options.SerializerSettings.Converters.Add(new StringEnumConverter())
                );
            services.AddHealthChecks();
            services.AddMediatR(typeof(CabUserService.AppSettings));
        }
    }
}