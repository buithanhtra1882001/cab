using CabPostService.Models.Entities.Base;

namespace CabPostService.Models.Entities
{
    public class ImageComment: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid ImageId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public int UpvotesCount { get; set; }
        public int DownvotesCount { get; set; }
        public int Status { get; set; }
    }
}
