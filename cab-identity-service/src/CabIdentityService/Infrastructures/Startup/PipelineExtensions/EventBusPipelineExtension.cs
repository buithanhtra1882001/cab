using Microsoft.AspNetCore.Builder;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using WCABNetwork.Cab.IdentityService.IntegrationEvents.EventHandlers;
using WCABNetwork.Cab.IdentityService.IntegrationEvents.Events;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Startup.PipelineExtensions
{
    public static class EventBusPipelineExtension
    {
        public static void UseEventBusSubcribers(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<UserDeletedIntegrationEvent, UserDeletedIntegrationEventHandler>();
        }
    }
}
