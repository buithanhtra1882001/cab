using CabPostService.Constants;
using CabPostService.Models.Entities.Base;

namespace CabPostService.Models.Entities
{
    public class CommentLike: BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CommentId { get; set; }
        public LikeType LikeType { get; set; }
    }
}
