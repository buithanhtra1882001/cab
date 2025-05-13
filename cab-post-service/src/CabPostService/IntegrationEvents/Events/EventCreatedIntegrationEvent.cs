using CAB.BuildingBlocks.EventBus.Events;
using Newtonsoft.Json;

namespace CabPostService.IntegrationEvents.Events
{
    public record EventCreatedIntegrationEvent : IntegrationEvent
    {
        public string AggregateId { get; private set; }
        public string AggregateName { get; private set; }
        public string Action { get; private set; }
        public string Data { get; private set; }
        public string CreatedBy { get; private set; }

        public EventCreatedIntegrationEvent(
            string aggregateId,
            EntityChangeType action,
            object entity)
            : this(aggregateId, entity.GetType().Name, action, entity)
        { }

        public EventCreatedIntegrationEvent(
            string aggregateId,
            string aggregateName,
            EntityChangeType action,
            object entity)
        {
            AggregateId = aggregateId;
            AggregateName = aggregateName;
            Action = action.ToString();
            Data = ToJsonString(entity);
            CreatedBy = string.Empty;
        }

        private string ToJsonString(object entity)
        {
            if (entity is null)
                return string.Empty;

            return JsonConvert.SerializeObject(entity, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Formatting = Formatting.None
            });
        }
    }

    public enum EntityChangeType : byte
    {
        Created = 0,
        Updated = 1,
        Deleted = 2
    }
}