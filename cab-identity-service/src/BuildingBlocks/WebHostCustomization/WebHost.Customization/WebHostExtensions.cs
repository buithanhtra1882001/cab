using Microsoft.EntityFrameworkCore;
using System;

namespace Microsoft.AspNetCore.Hosting
{
    public static class IWebHostExtensions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost, Action<TContext, IServiceProvider> seeder) where TContext : DbContext
        {
            EfMigrationUpdate.MigrateDbContext(webHost.Services, seeder);
            return webHost;
        }
    }
}