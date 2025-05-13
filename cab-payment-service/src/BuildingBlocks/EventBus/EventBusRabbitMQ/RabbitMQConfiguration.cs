using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAB.BuildingBlocks.EventBusRabbitMQ
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
