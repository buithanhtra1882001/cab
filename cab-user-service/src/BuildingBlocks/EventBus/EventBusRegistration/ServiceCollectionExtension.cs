using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Autofac;
using CAB.BuildingBlocks.EventBus;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CAB.BuildingBlocks.EventBusRabbitMQ;
using CAB.BuildingBlocks.EventBusServiceBus;
using RabbitMQ.Client;

namespace CAB.BuildingBlocks.EventBusRegistration
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
        {
            var subscriptionClientName = configuration["EventBus:SubscriptionClientName"];
            var exchangeName = configuration["EventBus:ExchangeName"];

            if (bool.TryParse(configuration["EventBus:AzureServiceBusEnabled"], out var azureServiceBusEnabled) && azureServiceBusEnabled)
            {
                services.AddSingleton<IServiceBusPersisterConnection>(sp =>
                {
                    var serviceBusConnectionString = configuration["EventBus:Connection"];
                    var serviceBusConnection = new ServiceBusConnectionStringBuilder(serviceBusConnectionString);

                    return new DefaultServiceBusPersisterConnection(serviceBusConnection, subscriptionClientName);
                });

                services.AddSingleton<IEventBus, EventBusServiceBus.EventBusServiceBus>(sp =>
                {
                    var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusServiceBus.EventBusServiceBus>>();
                    var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    var serviceBusConfiguration = new ServiceBusConfiguration()
                    {
                        AutofacScopeName = exchangeName
                    };
                    return new EventBusServiceBus.EventBusServiceBus(serviceBusPersisterConnection, logger, eventBusSubcriptionsManager,
                        iLifetimeScope, serviceBusConfiguration);
                });
            }
            else
            {
                services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = configuration["EventBus:Connection"],
                        DispatchConsumersAsync = true
                    };

                    if (!string.IsNullOrEmpty(configuration["EventBus:UserName"]))
                    {
                        factory.UserName = configuration["EventBus:UserName"];
                    }

                    if (!string.IsNullOrEmpty(configuration["EventBus:Password"]))
                    {
                        factory.Password = configuration["EventBus:Password"];
                    }

                    var retryCount = 5;

                    if (!string.IsNullOrEmpty(configuration["EventBus:RetryCount"]))
                    {
                        retryCount = int.Parse(configuration["EventBus:RetryCount"]);
                    }

                    return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
                });

                services.AddSingleton<IEventBus, EventBusRabbitMQ.EventBusRabbitMQ>(sp =>
                {
                    var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                    var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                    var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ.EventBusRabbitMQ>>();
                    var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                    var retryCount = 5;

                    if (!string.IsNullOrEmpty(configuration["EventBus:RetryCount"]))
                    {
                        retryCount = int.Parse(configuration["EventBus:RetryCount"]);
                    }

                    var ackWithoutWaitHandling = false;
                    if (!string.IsNullOrEmpty(configuration["EventBus:AckWithoutWaitHandling"]))
                    {
                        ackWithoutWaitHandling = bool.Parse(configuration["EventBus:AckWithoutWaitHandling"]);
                    }

                    var rabbitMQConfiguration = new RabbitMQConfiguration
                    {
                        AckWithoutWaitHandling = ackWithoutWaitHandling,
                        QueueName = subscriptionClientName,
                        RetryCount = retryCount,
                        ExchangeName = exchangeName,
                        AutofacScopeName = exchangeName
                    };
                    return new EventBusRabbitMQ.EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope,
                        eventBusSubscriptionsManager, rabbitMQConfiguration);
                });
            }

            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

            return services;
        }
    }
}