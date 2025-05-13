using AutoMapper;
using CabPaymentService.Controllers.Base;
using CabPaymentService.Infrastructures.Conventions;
using MediatR;
using Newtonsoft.Json.Converters;
using System.Text.Json;

namespace CabPaymentService.Infrastructures.Startup.ServicesExtensions
{
    public static class GeneralServiceExtension
    {
        public static void AddGeneralConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppSettings>(configuration);
            services.AddAutoMapper(typeof(AppSettings));

            services.AddCors(options =>
            {
                options.AddPolicy("CabCors", config =>
                {
                    config.AllowAnyOrigin();
                    config.AllowAnyMethod();
                    config.AllowAnyHeader();
                });
            });
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllers(options =>
            {
                options.Conventions.Add(new ApiControllerConvention(typeof(ApiController<>)));
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
            services.AddHealthChecks();
            services.AddMediatR(typeof(AppSettings));
        }
    }
}
