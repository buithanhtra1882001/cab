using MediatR;

namespace CabGroupService.Infrastructures.Startup.ServicesExtensions
{
    public static class GeneralServiceExtension
    {
        public static void AddGeneralConfigurations(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();
            services.Configure<AppSettings>(configuration);
            services.AddAutoMapper(typeof(AppSettings));
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddHealthChecks();
            services.AddMediatR(typeof(AppSettings));

            services.AddHttpContextAccessor();
        }
    }
}