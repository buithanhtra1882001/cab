namespace CabPaymentService.Model.Dtos
{
    public class RefundOrderDto
    {
        /// <summary>
        /// Mã giao dịch hoàn
        /// </summary>
        public string? OrderId { get; set; }
        /// <summary>
        /// Số tiền hoàn
        /// </summary>
        public string? Amount { get; set; }
        /// <summary>
        /// Kiểu hoàn tiền
        /// </summary>
        public string? RefundCategory { get; set; }
        /// <summary>
        /// Thời gian thanh toán
        /// </summary>
        public string? PayDate{ get; set; }
        /// <summary>
        /// email
        /// </summary>
        public string? Email { get; set; }
    }
}
