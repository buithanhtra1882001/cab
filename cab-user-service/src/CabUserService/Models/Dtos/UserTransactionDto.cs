using CabUserService.Constants;

namespace CabUserService.Models.Dtos
{
    public class UserTransactionDto
    {
        public double Amount { get; set; }
        public double PlatformFee { get; set; }
        public Guid? FromUserId { get; set; }
        public Guid ToUserId { get; set; }
        public string Description { get; set; }
        public string DonationMessage { get; set; } = "";
        public DonateType DonationType { get; set; } = DonateType.None;
        public Guid? PostId { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionType Type { get; set; }
    }
}
