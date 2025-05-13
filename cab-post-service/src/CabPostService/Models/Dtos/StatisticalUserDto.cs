namespace CabPostService.Models.Dtos
{
    public class StatisticalUserDto
    {
        public int like { get; set; }

        public int disLike { get; set; }

        public int comment { get; set; }

        public int newFriend { get; set; }

        public int post { get; set; }

        public bool isUpdateProfile { get; set; }

        public TopReaction topReaction { get; set; }

        public TopDonate topDonate { get; set; }
    }

    public class TopReaction
    {
        public string name { get; set; }

        public int count { get; set; }
    }

    public class TopDonate
    {
        public string name { get; set; }

        public int totalValue { get; set; }
    }
}
