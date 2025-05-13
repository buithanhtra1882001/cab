using CabPostService.Grpc.Procedures;
using CabPostService.Grpc.Protos.UserClient;
using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Queries;

namespace CabPostService.Handlers.User
{
    public partial class UserHandler :
        IQueryHandler<GetUserFriendIdsQuery, FriendIds?>
    {
        public async Task<FriendIds?> Handle(
            GetUserFriendIdsQuery request,
            CancellationToken cancellationToken)
        {
            var userService = _seviceProvider.GetRequiredService<IUserService>();
            var user = await userService.GetUserAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning($"Not found user with userId={request.UserId}");
                return null;
            }

            var userFriendIds = await userService.GetUserFriendIdsAsync(user.UserId);
            return userFriendIds;
        }
    }
}
