using CAB.BuildingBlocks.EventBus.Events;

namespace CabMediaService.IntegrationEvents.Events
{
    public record EventCreatedIntegrationEvent : IntegrationEvent
    {
        public string AggregateId { get; private set; }
        public string AggregateName { get; private set; }
        public string Action { get; private set; }
        public string Data { get; private set; }
        public string CreatedBy { get; private set; }

        public EventCreatedIntegrationEvent(string aggregateId, string aggregateName, string action, string data, string createdBy)
        {
            AggregateId = aggregateId;
            AggregateName = aggregateName;
            Action = action;
            Data = data;
            CreatedBy = createdBy;
        }
    }
}