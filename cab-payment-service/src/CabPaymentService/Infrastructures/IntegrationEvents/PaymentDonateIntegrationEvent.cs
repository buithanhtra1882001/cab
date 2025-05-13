using CAB.BuildingBlocks.EventBus.Events;

namespace CabPaymentService.Infrastructures.IntegrationEvents;

public record PaymentDonateIntegrationEvent() : IntegrationEvent
{
    public string ReceivingUserId { get; set; }
    public string Amount { get; set; }
}