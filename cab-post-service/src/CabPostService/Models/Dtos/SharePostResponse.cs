namespace CabPostService.Models.Dtos
{
    public class SharePostResponse
    {
        public Guid Id { get; set; }
        public string PostId { get; set; }
        public Guid UserId { get; set; }
        public string ShareLink { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsPublic { get; set; }
    }
}
