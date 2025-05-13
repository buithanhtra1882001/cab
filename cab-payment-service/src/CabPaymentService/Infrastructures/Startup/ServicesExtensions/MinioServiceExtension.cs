using Minio.AspNetCore;

namespace CabPaymentService.Infrastructures.Startup.ServicesExtensions
{
    public static class MinioServiceExtension
    {
        public static void AddMinioService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMinio(options =>
            {
                options.Endpoint = configuration.GetValue<string>("MinIOSettings:Endpoint");
                options.AccessKey = configuration.GetValue<string>("MinIOSettings:AccessKey");
                options.SecretKey = configuration.GetValue<string>("MinIOSettings:SecretKey");
            });
        }
    }
}
