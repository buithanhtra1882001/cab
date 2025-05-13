using CabPostService.Grpc.Protos.PostServer;
using CabPostService.Grpc.Protos.UserClient;
using Grpc.Core;

namespace CabPostService.Infrastructures.Startup.ServicesExtensions
{
    public static class GrpcServiceExtension
    {
        public static void AddGrpcConfigurations(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddGrpcClient<UserProtoService.UserProtoServiceClient>(o =>
            {
                o.Address = new Uri(configuration.GetValue<string>("UserService:BaseAddress"));
            })
            .ConfigureChannel(o =>
            {
                o.Credentials = ChannelCredentials.Insecure;
            });
        }
    }
}
