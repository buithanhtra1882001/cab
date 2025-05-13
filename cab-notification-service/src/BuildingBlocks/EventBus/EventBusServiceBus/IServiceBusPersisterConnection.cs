namespace CAB.BuildingBlocks.EventBusServiceBus
{
    using System;
    using Microsoft.Azure.ServiceBus;

    public interface IServiceBusPersisterConnection : IDisposable
    {
        ITopicClient TopicClient { get; }
        ISubscriptionClient SubscriptionClient { get; }
    }
}