using CabUserService.Constants;
using CabUserService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Entities
{
    public class User : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int SequenceId { get; set; }
        public long Credit { get; set; }
        public short Status { get; set; }
        public bool IsSoftDeleted { get; set; }
        public bool IsRequestCreator { get; set; }
        public bool IsVerifyEmail { get; set; } = false;
        public double Coin { get; set; }
        public bool CanReceiveDonation { get; set; }
        public UserType UserType { get; set; }
        public UserDetail UserDetail { get; set; }
    }
}