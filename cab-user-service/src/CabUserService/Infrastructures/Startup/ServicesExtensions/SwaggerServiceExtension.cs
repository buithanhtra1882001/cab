using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CabUserService.Infrastructures.Startup.ServicesExtensions
{
    public static class SwaggerServiceExtension
    {
        public static void AddSwaggerService(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "User Service", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }
    }
}