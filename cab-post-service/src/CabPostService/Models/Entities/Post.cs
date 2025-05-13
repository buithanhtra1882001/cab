using CabPostService.Constants;
using CabPostService.Models.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Entities
{
    public class Post : BaseEntity
    {
        [Key]
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public bool AdminBoost { get; set; }
        public string PostType { get; set; }
        public Guid CategoryId { get; set; }
        public string? Hashtags { get; set; }
        public string Content { get; set; }
        public string[]? ImageIds { get; set; }
        public string[]? VideoIds { get; set; }
        public int Point { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public int Status { get; set; }
        public bool IsDonateOpen { get; set; }
        public bool IsChecked { get; set; }
        public bool IsSoftDeleted { get; set; }
        public string Title { get; set; }
        public int ViewCount { get; set; }
        public int VoteUpCount { get; set; }
        public int VoteDownCount { get; set; }
        public decimal PosterScore { get; set; }
        public int? TrendingPoint { get; set; }
        public int? LegendaryPoint { get; set; }
        public int? OutstandingPoint { get; set; }
        public bool IsTopTrending { get; set; }
        public bool IsTopLegendary { get; set; }
        public bool IsOutstanding { get; set; }
        public int TotalReport { get; set; }

        public TypeShowPostEnum TypeShowPost { get; set; }

        public ICollection<PostVote>? Votes { get; set; }
        public string? GroupId { get; set; }
        public bool IsPersonalPost { get; set; } = true;
        public GroupPostStatus IsApproved { get; set; } = GroupPostStatus.APPROVED;
    }
}
