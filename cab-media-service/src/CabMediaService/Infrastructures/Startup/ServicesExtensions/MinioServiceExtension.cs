using Minio.AspNetCore;

namespace CabMediaService.Infrastructures.Startup.ServicesExtensions
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
                options.OnClientConfiguration = async client =>
                {
                    // Make a bucket on the server, if not already present.
                    var existed = await client.BucketExistsAsync(configuration.GetValue<string>("MinIOSettings:BucketName"));

                    if (!existed)
                    {
                        await client.MakeBucketAsync(configuration.GetValue<string>("MinIOSettings:BucketName")
                            , configuration.GetValue<string>("MinIOSettings:Region"));
                    }
                };
            });
        }
    }
}