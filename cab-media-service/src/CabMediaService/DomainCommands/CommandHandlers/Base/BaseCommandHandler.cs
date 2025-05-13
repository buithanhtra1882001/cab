using System.Reflection;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CabMediaService.Infrastructures.Exceptions;
using CabMediaService.IntegrationEvents.Events;
using MediatR;

namespace CabMediaService.DomainCommands.CommandHandlers.Base
{
    public abstract class BaseCommandHandler<TCommandHandler, TNotification> : INotificationHandler<TNotification>
        where TNotification : INotification
    {
        protected readonly ILogger<TCommandHandler> _logger;
        protected readonly IEventBus _eventBus;

        protected const string CREATED = "Created";
        protected const string UPDATED = "Updated";
        protected const string DELETED = "Deleted";

        protected BaseCommandHandler(ILogger<TCommandHandler> logger
            , IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        protected abstract Task<IEnumerable<EventCreatedIntegrationEvent>> Handle(TNotification notification);

        public async Task Handle(TNotification notification, CancellationToken cancellationToken)
        {
            var events = Enumerable.Empty<EventCreatedIntegrationEvent>();

            // Execute command handler in derived class
            try
            {
                events = await Handle(notification);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error on {typeof(TCommandHandler).Name}.{MethodBase.GetCurrentMethod().Name}");
                throw new CommandHandlerException(ex.Message);
            }

            // Save events
            try
            {
                foreach (var item in events)
                {
                    _eventBus.Publish(item);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error on {this.GetType().Name}.{MethodBase.GetCurrentMethod().Name}");
                throw new CommandHandlerException(ex.Message);
            }
        }
    }
}