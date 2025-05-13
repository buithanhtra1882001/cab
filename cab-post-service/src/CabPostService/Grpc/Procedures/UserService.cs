using AutoMapper;
using CabPostService.Grpc.Protos.PostServer;
using CabPostService.Grpc.Protos.UserClient;
using CabPostService.Models.Dtos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using System.Net;

namespace CabPostService.Grpc.Procedures
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        private readonly UserProtoService.UserProtoServiceClient _client;
        public UserService(ILogger<UserService> logger, IMapper mapper, IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            //var channel = GrpcChannel.ForAddress(configuration.GetValue<string>("UserService:BaseAddress"));
            //_client = new UserProtoService.UserProtoServiceClient(channel);

            var httpClientHandler = new HttpClientHandler();
            var channel = GrpcChannel.ForAddress(configuration.GetValue<string>("UserService:BaseAddress"), new GrpcChannelOptions
            {
                HttpClient = new HttpClient(httpClientHandler)
                {
                    DefaultRequestVersion = HttpVersion.Version20
                }
            });
            _client = new UserProtoService.UserProtoServiceClient(channel);

        }

        public async Task<UserResponse> GetUserAsync(Guid userId)
        {
            var request = new GetUserRequest
            {
                UserId = userId.ToString()
            };

            try
            {
                var user = await _client.GetUserAsync(request);
                var response = _mapper.Map<UserResponse>(user);
                return response;

            }
            catch (RpcException ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<FriendIds> GetUserFriendIdsAsync(Guid userId)
        {
            var request = new GetUserFriendIdsRequest
            {
                UserId = userId.ToString()
            };
            try
            {
                var response = await _client.GetUserFriendIdsAsync(request);
                return response;
            }
            catch (RpcException ex)
            {

                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }


        public async Task<UserWeightConstantsResponse> GetWeightConstantsAsync()
        {
            var request = new Empty();
            try
            {
                var response = await _client.GetWeightConstantsAsync(request);
                return response;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
