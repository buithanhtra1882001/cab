using CabMediaService.Infrastructures.Common;
using CabMediaService.Integration.Aws;
using CabMediaService.Integration.Configs;

namespace CabMediaService.Infrastructures.Startup.ServicesExtensions
{
    public static class AWSServiceExtension
    {
        public static void AddAWSService(this IServiceCollection services, IConfiguration configuration)
        {
            var awsConfig = configuration.GetSection(nameof(AwsConfig)).Get<AwsConfig>();
            services.AddSingleton(awsConfig);
            services.AddTransient<AwsS3IntegrationHelper>();
            services.AddScoped<IS3Client, S3Client>(sp =>
            {
                return new S3Client(awsConfig);
            });
        }
    }
}
