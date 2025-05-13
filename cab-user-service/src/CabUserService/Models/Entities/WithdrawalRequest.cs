using CabUserService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Entities
{
    public class WithdrawalRequest : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public User User { get; set; }
        public double WithdrawalAmount  { get; set; }
        public WithdrawalRequestStatus Status { get; set; }
        public string StatusDescription { get; set; }
    }

    public enum WithdrawalRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Declined = -1,
    }
}
