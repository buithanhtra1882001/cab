using CabPostService.Grpc.Protos.PostServer;
using CabPostService.Grpc.Protos.UserClient;
using CabPostService.Models.Dtos;

namespace CabPostService.Grpc.Procedures
{
    public interface IUserService
    {
        Task<UserResponse> GetUserAsync(Guid userId);
        Task<FriendIds> GetUserFriendIdsAsync(Guid userId);
        Task<UserWeightConstantsResponse> GetWeightConstantsAsync();
    }
}
