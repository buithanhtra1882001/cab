using CabUserService.Constants;

namespace CabUserService.Models.Dtos
{
    public class AddBalanceRequest
    {
        public Guid UserId { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public BalanceType Type { get; set; }
    }
}
