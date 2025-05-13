using AutoMapper;
using CabUserService.Grpc.Profos.UserServer;
using CabUserService.Infrastructures.Communications.Http;
using CabUserService.Infrastructures.Repositories.Interfaces;
using Grpc.Core;
using System.Net;

namespace CabUserService.Grpc.Procedures
{
    public class UserService : UserProtoService.UserProtoServiceBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IUserFriendRepository _userFriendRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;

        public UserService(ILogger<UserService> logger,
                           IUserRepository userRepository,
                           IUserDetailRepository userDetailRepository,
                           IUserFriendRepository userFriendRepository,
                           IMapper mapper)
        {
            _logger = logger;
            _userRepository = userRepository;
            _userDetailRepository = userDetailRepository;
            _userFriendRepository = userFriendRepository;
            _mapper = mapper;
        }

        public override async Task<UserModel> GetUserProfile(GetUserProfileRequest request, ServerCallContext context)
        {
            var userId = Guid.Parse(request.UserId);
            var user = await _userRepository.GetByIdAsync(userId);
            var userDetail = await _userDetailRepository.GetByIdAsync(userId);

            if (userDetail is null)
            {
                var errorMessage = $"Not found user with Id ={request.UserId}";
                _logger.LogError(errorMessage);
                throw new Exception(errorMessage);
            }

            var result = new UserModel();
            _mapper.Map(user, result);
            _mapper.Map(userDetail, result);
            return result;
        }

        public override async Task<FriendIds> GetUserFriendIds(GetUserFriendIdsRequest request, ServerCallContext context)
        {
            var userId = Guid.Parse(request.UserId);
            var friendIds = (await _userFriendRepository.GetListAsync(item => item.UserId == userId))
                          .Select(item => item.FriendId.ToString())
                          .ToList();
            var result = new FriendIds();
            result.Ids.Add(friendIds);
            return result;
        }
    }
}
