using System.Text.Json.Serialization;

namespace CabPostService.Models.Dtos
{
    public class GetAllPostResponse
    {
        public string Id { get; set; }
        public string UserAvatar { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public string PostType { get; set; }
        public Guid CategoryId { get; set; }
        public string Hashtags { get; set; }
        public string Content { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsDonateOpen { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public int ViewCount { get; set; }
        public int VoteUpCount { get; set; }
        public int VoteDownCount { get; set; }
        public int VoteType { get; set; }
        public bool CurrentUserHasVoteUp { get; set; }
        public bool CurrentUserHasVoteDown { get; set; }
        public List<PostImageResponse> PostImageResponses { get; set; }
        public List<PostVideoResponse> PostVideoResponses { get; set; }

        [JsonIgnore]
        public PagingResponse<UserCommentResponse> UserCommentResponses { get; set; }
    }
}
