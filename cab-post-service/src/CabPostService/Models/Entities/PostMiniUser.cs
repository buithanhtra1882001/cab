namespace CabPostService.Models.Entities
{
    public class PostMiniUser : User
    {
        public ICollection<SharePost> SharePosts { get; set; }
    }
}
