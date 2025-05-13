using CabUserService.Constants;
using CabUserService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Entities
{
    public class UserTransaction : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
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
        public bool IsHidingMessage { get; set; }
    }
}
