using CabUserService.Models.Entities.Base;

namespace CabUserService.Models.Entities
{
    public class UserFriend : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
    }
}
