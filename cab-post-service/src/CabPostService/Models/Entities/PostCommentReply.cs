using CabPostService.Models.Entities.Base;

namespace CabPostService.Models.Entities
{
    public class PostCommentReply : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid CommentId { get; set; }
        public Guid? ParentReplyId { get; set; }
        public int ReplyLevel { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = "";
        public int UpvotesCount { get; set; }
        public int DownvotesCount { get; set; }
        public int Status { get; set; }
    }
}
