namespace CabPostService.Models.Dtos
{
    public class AdminGetPostsResponse
    {
        public string Id { get; set; }
        public string UserAvatar { get; set; }
        public Guid UserId { get; set; }
        public string UserFullName { get; set; }
        public string PostType { get; set; }
        public Guid CategoryId { get; set; }
        public List<string> Hashtags { get; set; }
        public string Content { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<string> VideoUrls { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsDonateOpen { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
    }
}
