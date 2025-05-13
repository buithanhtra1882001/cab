namespace CabUserService.Models.Dtos
{
    public class StatisticalDonateDto
    {
        public TopMoney TopMoney { get; set; }

        public TopAction TopAction { get; set; }
    }

    public class TopMoney
    {
        public string TopDonaterValueName { get; set; }

        public long TopDonaterValue { get; set; }
    }

    public class TopAction
    {
        public string TopDonaterTheMostTimes { get; set; }

        public int TopDonaterTheMostValue { get; set; }
    }
}
