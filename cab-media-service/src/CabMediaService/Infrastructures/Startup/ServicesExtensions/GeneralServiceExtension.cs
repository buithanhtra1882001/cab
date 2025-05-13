using AutoMapper;
using CabMediaService.Controllers.Base;
using CabMediaService.Infrastructures.Conventions;
using Newtonsoft.Json.Converters;
using MediatR;

namespace CabMediaService.Infrastructures.Startup.ServicesExtensions
{
    public static class GeneralServiceExtension
    {
        public static void AddGeneralConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration);
            services.AddAutoMapper(typeof(CabMediaService.AppSettings));

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers(options =>
                {
                    options.Conventions.Add(new ApiControllerConvention(typeof(ApiController<>)));
                })
                .AddNewtonsoftJson(options =>
                     options.SerializerSettings.Converters.Add(new StringEnumConverter())
                );
            services.AddHealthChecks();
            services.AddMediatR(typeof(CabMediaService.AppSettings));
        }
    }
}