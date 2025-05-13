namespace CabUserService.Models.Dtos
{
    public class InteractionStatisticsResponse
    {
        public long TotalProfileView { get; set; }

        public long TotalFollow { get; set; }

        public double TotalDonationAmount { get; set; }
    }
}
