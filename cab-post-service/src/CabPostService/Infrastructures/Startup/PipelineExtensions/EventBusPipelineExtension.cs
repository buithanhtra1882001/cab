using CabPostService.IntegrationEvents.EventHandlers;
using CabPostService.IntegrationEvents.Events;
using CAB.BuildingBlocks.EventBus.Abstractions;

namespace CabPostService.Infrastructures.Startup.PipelineExtensions
{
    public static class EventBusPipelineExtension
    {
        public static void UseEventBusSubcribers(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.Subscribe<UserProfileCreatedIntegrationEvent, UserProfileCreatedIntegrationEventHandler>();
            eventBus.Subscribe<UserProfileUpdatedIntegrationEvent, UserProfileUpdatedIntegrationEventHandler>();
            eventBus.Subscribe<UserProfileUpdateAvatarIntegrationEvent,  UserProfileUpdateAvatarIntegrationEventHandler>();
        }
    }
}