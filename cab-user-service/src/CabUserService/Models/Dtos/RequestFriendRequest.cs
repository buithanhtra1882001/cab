using CabUserService.Constants;

namespace CabUserService.Models.Dtos
{
    public class RequestFriendRequest
    {
        public Guid UserId { get; set; }

        public Guid RequestUserId { get; set; }

        public ACTION_FRIEND_TYPE StatusAction { get; set; }

        public REQUEST_TYPE TypeRequest { get; set; }
    }
}
