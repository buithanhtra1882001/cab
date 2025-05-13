using CabUserService.Constants;
using CabUserService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Models.Entities
{
    public class UserBalanceLog : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public BalanceType Type { get; set; }
    }
}
