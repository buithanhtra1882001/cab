namespace CabPaymentService.Model.Dtos
{
    /// <summary>
    /// Object client gửi lên để cập nhật thông tin thanh toán
    /// </summary>
    public class UpdateOrderInfoDto : VnPayBaseDto
    {
        /// <summary>
        /// Số giao dịch ngân hàng
        /// </summary>
        public string? vnp_BankTranNo { get; set; }
        /// <summary>
        /// Loại thẻ thanh toán
        /// </summary>
        public string? vnp_CardType { get; set; }
    }
}
