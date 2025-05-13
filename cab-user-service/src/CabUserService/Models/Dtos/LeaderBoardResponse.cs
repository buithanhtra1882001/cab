namespace CabUserService.Models.Dtos
{
    public class LeaderBoardResponse
    {
        public List<StatsticUserFollow> UserFollows { get; set; }
        public List<StatsticUserDonate> UserDonates { get; set; }
        public List<StatsticUserDonate> UserRecieveDonates { get; set; }
    }

    public class StatsticUserFollow
    {
        public Guid UserId { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }

        public decimal TotalFollow { get; set; }
    }

    public class StatsticUserDonate
    {
        public Guid UserId { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public double TotalAmount { get; set; }
    }
}
