using CAB.BuildingBlocks.EventBus.Abstractions;
using CabUserService.IntegrationEvents.EventHandlers;
using CabUserService.IntegrationEvents.Events;

namespace CabUserService.Infrastructures.Startup.PipelineExtensions
{
    public static class EventBusPipelineExtension
    {
        public static void UseEventBusSubcribers(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            
            eventBus.Subscribe<UserCreatedIntegrationEvent, UserCreatedIntegrationEventHandler>();
            eventBus.Subscribe<UserRegisterIntegrationEvent, UserRegisterIntegrationEventHandler>();
        }
    }
}