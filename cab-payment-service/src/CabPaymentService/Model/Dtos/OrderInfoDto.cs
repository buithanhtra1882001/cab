using CabPaymentService.Infrastructures.Constants;
using Newtonsoft.Json;

namespace CabPaymentService.Model.Dtos
{
    public class OrderInfoDto
    {
        public OrderInfoDto()
        {
            OrderId = Guid.NewGuid();
            Category = "other";
        }

        [JsonIgnore]
        public Guid OrderId { get; set; }
        public long Amount { get; set; }

        public string Locale { get; set; }

        [JsonIgnore]
        public string Category { get; set; }
        public BillInfoDto BillInfo { get; set; }
        public InvoiceDto Invoice { get; set; }
        /// <summary>
        /// loại giao dịch:
        /// TOP_UP: nạp tiền vào tài khoản
        /// DONATE: donate tiền cho content creator
        /// COMMISSION: cắt phế
        /// </summary>
        public VnPayTransactionType TransactionType { get; set; }

        /// <summary>
        /// tài khoản đích nhận tiền.
        /// Trùng với userid với loại giao dịch là TOP_UP
        /// Khác với uerid với loại giao dịch là DONATE
        /// </summary>
        public Guid PaymentReceiver { get; set; }

        public Guid CreatedBy { get; set; }
    }
}
