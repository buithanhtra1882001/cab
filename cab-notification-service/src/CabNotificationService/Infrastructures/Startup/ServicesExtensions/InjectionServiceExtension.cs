using CabNotificationService.Infrastructures.Communications.Http;
using CabNotificationService.Infrastructures.DbContexts;
using CabNotificationService.Infrastructures.Repositories;
using CabNotificationService.Infrastructures.Repositories.Interfaces;
using CabNotificationService.IntegrationEvents.EventHandlers;

namespace CabNotificationService.Infrastructures.Startup.ServicesExtensions
{
    public static class InjectionServiceExtension
    {
        public static void AddInjectedServices(this IServiceCollection services)
        {
            //services.AddSingleton<CassandraDbContext>();
            services.AddSingleton<ScyllaDbContext>();
            services.AddHttpClient<IHttpClientWrapper, HttpClientWrapper>();

            services.AddTransient<INotificationRepository, NotificationRepository>();
            services.AddTransient<INotificationUserIdMaterializedViewRepository, NotificationUserIdMaterializedViewRepository>();

            services.AddTransient<NotificationIntegrationEventHandler>();
        }
    }
}