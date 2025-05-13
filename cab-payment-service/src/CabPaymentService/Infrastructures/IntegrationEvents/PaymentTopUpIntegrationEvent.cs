using CAB.BuildingBlocks.EventBus.Events;

namespace CabPaymentService.Infrastructures.IntegrationEvents;

public record PaymentTopUpIntegrationEvent : IntegrationEvent
{
    public string ReceivingUserId { get; set; }
    public string Amount { get; set; }
}