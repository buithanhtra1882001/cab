using CabUserService.Models.Entities;

namespace CabUserService.Models.Dtos
{
    public class WithdrawalRequestResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public double WithdrawalAmount { get; set; }
        public WithdrawalRequestStatus Status { get; set; }
        public string StatusDescription { get; set; }
    }
}
