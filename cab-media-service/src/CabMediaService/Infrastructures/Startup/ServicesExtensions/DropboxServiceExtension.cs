using CabMediaService.Integration.Dropbox;
using CabMediaService.Services.Interfaces;

namespace CabMediaService.Infrastructures.Startup.ServicesExtensions
{
    public static class DropboxServiceExtension
    {
        public static void AddDropboxService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IDropboxService, DropboxService>();
        }
    }
}
