using CabPostService.Grpc.Protos.UserClient;
using CabPostService.Handlers.Interfaces;

namespace CabPostService.Models.Queries
{
    public class GetUserWeightConstantsQuery: IQuery<UserWeightConstantsResponse>
    {
    }
}
