namespace CabPostService.Models.Dtos
{
    public class StatisticalDonateDto
    {        
        public List<TopMoney> TopMoney { get; set; }

        public List<TopAction> TopAction { get; set; }
    }

    public class TopMoney
    {
        public Guid UserId { get; set; }

        public string Avatar { get; set; }

        public string UserName { get; set; }

        public double TotalAmount { get; set; }
    }

    public class TopAction
    {
        public Guid UserId { get; set; }

        public string Avatar { get; set; }

        public string UserName { get; set; }

        public int TotalAction { get; set; }
    }
}
