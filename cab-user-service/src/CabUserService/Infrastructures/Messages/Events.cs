namespace CabCommon.Infrastructures.Messages.Events;
/// <summary>
/// Result of command SubtractCoin, send from User service, after successfully subtract user coin
/// </summary>
public interface CoinSubtracted
{
    public Guid RefundRequestId { get; set; }
}
/// <summary>
/// Result of command SubtractCoin, send from User service, when user coin less than requested refund amount
/// </summary>
public interface InvalidCoinAmount
{
    public Guid RefundRequestId { get; set; }
}
/// <summary>
/// Initial Event to initiate Refund saga
/// </summary>
public interface RefundRequested
{
    /// <summary>
    /// Id of current refund request
    /// </summary>
    public Guid RefundRequestId { get; set; }
    /// <summary>
    /// Id of previously completed transaction being refunded
    /// </summary>
    public Guid TransactionId { get; set; }
    /// <summary>
    /// số tiền yêu cầu hoàn khi bắt đầu saga
    /// </summary>
    public long Amount { get; set; }
    /// <summary>
    /// số coin quy đổi từ số tiền yêu cầu hoàn lại
    /// </summary>
    public long CoinAmount { get; set; }

    /// <summary>
    /// Id of user to subtract coin
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// IP user to send to vnpay as querystring in refund request
    /// </summary>
    public string IpAddress { get; set; }
}
public interface RefundRequestCreated
{
    public Guid RefundRequestId { get; set; }
    public Guid TransactionId { get; set; }
    public long CoinAmount { get; set; }
    public long Amount { get; set; }
    public Guid UserId { get; set; }
    public string IpAddress { get; set; }
}
public interface VnpayRefundSuccess
{
    public Guid RefundRequestId { get; set; }
}
public interface VnpayRefundFail
{
    public Guid RefundRequestId { get; set; }
    public string Reason { get; set; }
}
public interface CoinSubtractFail
{
    public Guid RefundRequestId { get; set; }
}