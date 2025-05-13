using CabUserService.Constants;
using CabUserService.Models.Entities;

namespace CabUserService.Models.Dtos
{

    public class UserRequestFriendDto
    {
        public Guid RequestUserId { get; set; }
        public string RequestFullName { get; set; }
        public string Avatar { get; set; }
        public int RequestType { get; set; }
        public bool IsFollow { get; set; }
    }
}
