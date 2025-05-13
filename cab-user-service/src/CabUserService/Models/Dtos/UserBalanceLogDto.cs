using CabUserService.Constants;

namespace CabUserService.Models.Dtos
{
    public class UserBalanceLogDto
    {
        public decimal Amount { get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public BalanceType Type { get; set; }
    }
}
