namespace CabPostService.Models.Dtos
{
    public class UserCommentResponse
    {
        public Guid Id { get; set; }
        public string PostId { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public string Avatar { get; set; }
        public string Content { get; set; }
        public int UpvotesCount { get; set; }
        public int DownvotesCount { get; set; }
        public int Status { get; set; }
        public PagingResponse<UserReplyResponse> Replies { get; set; }
        public long TotalReply { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool CurrentUserHasLike{ get; set; }
        public bool CurrentUserHasUnlike { get; set; }
    }
}
