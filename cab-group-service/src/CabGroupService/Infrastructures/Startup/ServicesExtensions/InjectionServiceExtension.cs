using CabGroupService.Infrastructures.Communications.Http;
using CabGroupService.Infrastructures.DbContexts;
using CabGroupService.Infrastructures.Repositories;
using CabGroupService.Infrastructures.Repositories.Interfaces;
using CabGroupService.IntegrationEvents.EventHandlers;
using CabGroupService.Services;
using CabGroupService.Services.Interfaces;

namespace CabGroupService.Infrastructures.Startup.ServicesExtensions
{
    public static class InjectionServiceExtension
    {
        public static void AddInjectedServices(this IServiceCollection services)
        {
            services.AddDbContext<GroupDbContext>();
            services.AddHttpClient<IHttpClientWrapper, HttpClientWrapper>();

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<IGroupMemberService, GroupMemberService>();
            
            services.AddTransient<NotificationIntegrationEventHandler>();
        }
    }
}