namespace CabCommon.Infrastructures.Messages.Commands;

/// <summary>
/// Command send to User service to rollback saga coin on refund saga fail
/// </summary>
public interface RollbackCoin
{
    public Guid RefundRequestId { get; set; }
    /// <summary>
    /// Id of user to subtract coin
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Amount of coin to subtract
    /// </summary>
    public long CoinAmount { get; set; }
}
/// <summary>
/// Command send to User service to subtract user's coin in refund saga
/// </summary>
public interface SubtractCoin
{
    /// <summary>
    /// Amount of coin to subtract
    /// </summary>
    public long CoinAmount { get; set; }
    /// <summary>
    /// Id of user to subtract coin
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// Id of current refund request
    /// </summary>
    public Guid RefundRequestId { get; set; }
}
public interface RequestRefundOnVnpay
{
    public Guid RefundRequestId { get; set; }
    /// <summary>
    /// Amount to request refund from VnPay
    /// </summary>
    public long Amount { get; set; }
    /// <summary>
    /// Id of completed transaction being refunded
    /// </summary>
    public Guid TransactionId { get; set; }
    /// <summary>
    /// Ip of user
    /// </summary>
    public string IpAddress { get; set; }
}
public interface UpdateStatusRefundRequest
{
    public Guid RefundRequestId { get; set; }
    /// <summary>
    /// SUCCESS or FAIL
    /// </summary>
    public bool IsSuccess { get; set; }
    public string Reason { get; set; }
}
/// <summary>
/// send to User service to add coin to user after buy coin transaction
/// </summary>
public interface AddCoin
{
    /// <summary>
    /// User Id to add coin
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// amount of coin to be added
    /// </summary>
    public long CoinAmount { get; set; }
}