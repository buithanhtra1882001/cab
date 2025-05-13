using CabPostService.Grpc.Procedures;
using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;

namespace CabPostService.Handlers.User
{
    public partial class UserHandler :
        IQueryHandler<GetUserByIdQuery, UserResponse?>
    {
        public async Task<UserResponse?> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken)
        {
            var userService = _seviceProvider.GetRequiredService<IUserService>();
            var user = await userService.GetUserAsync(request.UserId);
            if (user is null)
            {
                _logger.LogWarning($"Not found user with userId={request.UserId}");
                return null;
            }
            return user;
        }
    }
}
