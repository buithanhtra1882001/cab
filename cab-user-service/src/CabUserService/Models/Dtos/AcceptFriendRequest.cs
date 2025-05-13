using CabUserService.Constants;

namespace CabUserService.Models.Dtos
{
    public class AcceptFriendRequest
    {
        public Guid UserId { get; set; }
        public Guid RequestUserId { get; set; }
        public ACCEPTANCE_STATUS AcceptStatus { get; set; }
    }
}
