using CabUserService.Models.Entities;
using MailKit;

namespace CabUserService.Models.Dtos
{
    public class StatisticalUserDto
    {
        public int Like { get; set; }

        public int DisLike { get; set; }

        public int Comment { get; set; }

        public int NewFriend { get; set; }

        public bool IsUpdateProfile { get; set; }

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
