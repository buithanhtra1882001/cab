using CabPostService.Grpc.Procedures;
using CabPostService.Grpc.Protos.UserClient;
using CabPostService.Handlers.Interfaces;
using CabPostService.Models.Queries;

namespace CabPostService.Handlers.User
{
    public partial class UserHandler :
        IQueryHandler<GetUserWeightConstantsQuery, UserWeightConstantsResponse>
    {
        public async Task<UserWeightConstantsResponse> Handle(
            GetUserWeightConstantsQuery request,
            CancellationToken cancellationToken)
        {
            var userService = _seviceProvider.GetRequiredService<IUserService>();
            var response = await userService.GetWeightConstantsAsync();
            return response;
        }
    }
}
