using CabGroupService.IntegrationEvents.EventHandlers;
using CabGroupService.IntegrationEvents.Events;
using CAB.BuildingBlocks.EventBus.Abstractions;

namespace CabGroupService.Infrastructures.Startup.PipelineExtensions
{
    public static class EventBusPipelineExtension
    {
        public static void UseEventBusSubcribers(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<NotificationIntegrationEvent, NotificationIntegrationEventHandler>();
        }
    }
}