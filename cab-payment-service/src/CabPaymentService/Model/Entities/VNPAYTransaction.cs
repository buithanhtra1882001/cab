using CabPaymentService.Infrastructures.Constants;
using CabPaymentService.Model.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabPaymentService.Model.Entities
{
    public class VnPayTransaction : BaseEntity
    {
        /// <summary>
        /// dùng trong querystring vnp_TxnRef trong request/response với VnPay
        /// Mã tham chiếu của giao dịch tại hệ thống của Cabvn. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày. Ví dụ: 23554
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        public long Amount { get; set; }
        public string? OrderDesc { get; set; }

        public DateTime TransactionCreatedDate { get; set; }
        /// <summary>
        /// 0: trạng thái thanh toán của giao dịch chưa có IPN lưu tại hệ thống của merchant chiều khởi tạo URL thanh toán.
        /// 1: Trạng thái thanh toán thành công, được set khi VNPay gọi IPN URL
        /// 2: Trạng thái thanh toán thất bại / lỗi, được set khi VNPay gọi IPN URL
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Tương ứng với querystring vnp_TransactionNo trong request/response với VnPay.
        /// Là mã giao dịch ghi nhận tại hệ thống VNPAY. Ví dụ: 20170829153052
        /// </summary>
        public long PaymentTranId { get; set; } = 0;
        public string? BankCode { get; set; }
        public string? PayStatus { get; set; }
        public string? Locale { get; set; }
        public string? Category { get; set; }
        public string? ExpireDate { get; set; }
        public string? ResponseCode { get; set; } // not in orderinfodto

        public Guid BillId { get; set; }  // not in orderinfodto
        public BillInfo BillInfo { get; set; }
        public Guid InvoiceId { get; set; }  // not in orderinfodto
        public Invoice Invoice { get; set; }
        /// <summary>
        /// loại giao dịch:
        /// DEPOSIT: nạp tiền vào tài khoản
        /// DONATE: donate tiền cho content creator
        /// </summary>
        public VnPayTransactionType TransactionType { get; set; }

        public Guid PaymentReceiver { get; set; }
        public PaymentCommission? PaymentCommission { get; set; }
    }
}
