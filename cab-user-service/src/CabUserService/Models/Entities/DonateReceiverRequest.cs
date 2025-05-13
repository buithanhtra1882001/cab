using CabUserService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Entities
{
    public class DonateReceiverRequest : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public User User { get; set; }
        public string NationalId { get; set; }
        public string ReferenceLinks { get; set; }
        public string BankAccount {  get; set; }
        public string BankName { get; set; }
        public DonateReceiverRequestStatus Status { get; set; }
    }

    public enum DonateReceiverRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Invalid = -1,
    }
}
