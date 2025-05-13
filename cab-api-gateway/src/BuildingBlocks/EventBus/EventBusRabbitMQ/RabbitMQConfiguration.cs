namespace Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ
{
    public class RabbitMQConfiguration
    {
        public string AutofacScopeName { get; set; }
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public int RetryCount { get; set; }
        public bool AckWithoutWaitHandling { get; set; }
    }
}