using CabPostService.Constants;
using CabPostService.Models.Entities.Base;

namespace CabPostService.Models.Entities
{
    public class UserBehavior:BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PostId { get; set; }
        public UserActionType Type { get; set; }
        public bool IsHidden { get; set; }
    }
}
