using CabMediaService.BackgroundServices;
using CabMediaService.Infrastructures.DbContexts;
using CabMediaService.Infrastructures.Repositories;
using CabMediaService.Infrastructures.Repositories.Interfaces;
using CabMediaService.Services;
using CabMediaService.Services.Interfaces;

namespace CabMediaService.Infrastructures.Startup.ServicesExtensions
{
    public static class InjectionServiceExtension
    {
        public static void AddInjectedServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddSingleton<CassandraDbContext>();
            services.AddSingleton<ScyllaDbContext>();
            services.AddDbContext<PostgresDbContext>();
            services.AddHttpClient();

            services.AddTransient<IMediaImageRepository, MediaImageRepository>();
            services.AddTransient<IAWSMediaService, AWSMediaService>();
            services.AddTransient<IDropBoxMediaService, DropBoxMediaService>();
            services.AddHostedService<CleanService>();
        }
    }
}