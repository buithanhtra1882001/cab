namespace CabPaymentService.Model.Dtos
{
    public class VnPayBaseDto
    {
        /// <summary>
        /// Mã merchant
        /// </summary>
        public string? vnp_TmnCode { get; set; }
        /// <summary>
        /// Ma đơn hàng lưu ở db merchant gửi VNPAY
        /// </summary>
        public string? vnp_TxnRef { get; set; }
        /// <summary>
        /// Ma GD tai he thong VNPAY
        /// </summary>
        public string? vnp_TransactionNo { get; set; }
        /// <summary>
        /// Tình trạng của giao dịch tại Cổng thanh toán VNPAY. Xem thêm tại bảng mã lỗi
        /// </summary>
        public string? vnp_TransactionStatus { get; set; }
        /// <summary>
        /// Ngày thanh toán
        /// </summary>
        public string? vnp_PayDate { get; set; }
        /// <summary>
        /// Số xèng
        /// </summary>
        public string? vnp_Amount { get; set; }
        /// <summary>
        /// Mã ngân hàng
        /// </summary>
        public string? vnp_BankCode { get; set; }
        /// <summary>
        /// Thông tin đơn hàng
        /// </summary>
        public string? vnp_OrderInfo { get; set; }
        /// <summary>
        /// Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
        /// </summary>
        public string? vnp_ResponseCode { get; set; }
        /// <summary>
        /// HmacSHA512 cua du lieu tra ve
        /// </summary>
        public string? vnp_SecureHash { get; set; }
        /// <summary>
        /// 	Giải thuật checksum sử dụng. Phiên bản hiện tại hỗ trợ SHA256, HMACSHA512
        /// </summary>
        public string? vnp_SecureHashType { get; set; }

    }
}
