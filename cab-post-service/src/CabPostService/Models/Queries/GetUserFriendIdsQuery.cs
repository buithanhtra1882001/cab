using CabPostService.Grpc.Protos.UserClient;
using CabPostService.Handlers.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CabPostService.Models.Queries
{
    public class GetUserFriendIdsQuery : IQuery<FriendIds?>
    {
        [Required]
        public Guid UserId { get; set; }
    }
}
