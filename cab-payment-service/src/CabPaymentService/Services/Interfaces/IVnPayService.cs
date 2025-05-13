using CabPaymentService.Model.Dtos;

namespace CabPaymentService.Services.Interfaces
{
    public interface IVnPayService
    {
        Task<string> CreatePayment(OrderInfoDto order, string IpAddress);
        public Task<QueryTransactionResponseDto> QueryTransactionFromVnPay(Guid transId, string IpAddress);

        Task<string> IpnCheck(UpdateOrderInfoDto infoDto);
    }
}
