using CabNotificationService.IntegrationEvents.EventHandlers;
using CabNotificationService.IntegrationEvents.Events;
using CAB.BuildingBlocks.EventBus.Abstractions;

namespace CabNotificationService.Infrastructures.Startup.PipelineExtensions
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