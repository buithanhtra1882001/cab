using System.ComponentModel.DataAnnotations;

namespace CabPaymentService.Model.Dtos
{
    /// <summary>
    /// Object nhận về từ VnPay khi truy vấn kết quả thanh toán command=querydr
    /// </summary>
    public class QueryTransactionResponseDto : VnPayBaseDto
    {
        /// <summary>
        /// Mô tả thông tin tương ứng với vnp_ResponseCode
        /// </summary>
        public string? vnp_Message { get; set; }

        /// <summary>
        /// Loại giao dịch tại hệ thống VNPAY:
        /// 01: GD thanh toán
        /// 02: Giao dịch hoàn trả toàn phần
        /// 03: Giao dịch hoàn trả một phần
        /// </summary>
        public string? vnp_TransactionType { get; set; }

    }
}
