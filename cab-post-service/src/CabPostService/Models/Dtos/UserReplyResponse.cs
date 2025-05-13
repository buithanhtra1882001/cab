namespace CabPostService.Models.Dtos
{
    public class UserReplyResponse
    {
        public Guid Id { get; set; }
        public Guid CommentId { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public string Avatar { get; set; }
        public Guid? ParentReplyId { get; set; }
        public int ReplyLevel { get; set; }
        public string Content { get; set; }
        public int UpvotesCount { get; set; }
        public int DownvotesCount { get; set; }
        public int Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
