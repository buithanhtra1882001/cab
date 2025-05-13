using CAB.BuildingBlocks.EventBus.Abstractions;
using CabNotificationService.IntegrationEvents.Events;
using CabNotificationService.Models.Commands;
using MediatR;

namespace CabNotificationService.IntegrationEvents.EventHandlers
{
    public class NotificationIntegrationEventHandler :
        IIntegrationEventHandler<NotificationIntegrationEvent>
    {
        private readonly ILogger<NotificationIntegrationEventHandler> _logger;
        private readonly IMediator _mediator;

        public NotificationIntegrationEventHandler(ILogger<NotificationIntegrationEventHandler> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(NotificationIntegrationEvent @event)
        {
            try
            {
                _logger.LogInformation($"Consume NotificationIntegrationEvent with eventId {@event.Id} at {@event.CreationDate.ToString("dd-MM-yyyy HH:mm:ss")}");

                await _mediator.Publish(new CreateNotificationCommand
                {
                    UserIds = @event.UserIds,
                    Actor = @event.Actor,
                    ReferenceId = @event.ReferenceId,
                    NotificationType = @event.NotificationType,
                    DonateAmount = @event.DonateAmount,
                    Type = @event.Type,
                    ReferenceUrl = @event.ReferenceUrl
                });
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error at NotificationIntegrationEvent: {ex.Message}");
            }
        }
    }
}
