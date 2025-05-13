using CAB.BuildingBlocks.EventBus.Abstractions;

namespace CabMediaService.Infrastructures.Startup.PipelineExtensions
{
    public static class EventBusPipelineExtension
    {
        public static void UseEventBusSubcribers(this IApplicationBuilder app)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
        }
    }
}