using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace WebHost.Customization
{
    public static class HostExtensions
    {
        public static IHost MigrateDbContext<TContext>(this IHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            EfMigrationUpdate.MigrateDbContext(webHost.Services, seeder);
            return webHost;
        }
    }
}
