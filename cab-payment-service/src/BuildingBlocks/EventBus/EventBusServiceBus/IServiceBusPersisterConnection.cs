using Microsoft.Azure.ServiceBus;

namespace CAB.BuildingBlocks.EventBusServiceBus
{
    public interface IServiceBusPersisterConnection : IDisposable
    {
        ITopicClient TopicClient { get; }
        ISubscriptionClient SubscriptionClient { get; }
    }
}
