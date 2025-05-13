using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;

namespace CabUserService.Infrastructures.Repositories.Interfaces
{
    public interface IUserRepository : IPostgresBaseRepository<User>
    {
        Task<List<User>> GetAllAsync(GetAllUserRequest request);
        Task<List<User>> GetAllByUserName(Guid auid, string username);
        Task<long> GetTotalUser(GetAllUserRequest request);
        Task<int> UpdateTypeAsync(User entity, bool isConfimCrator);
        Task<List<UserRequestFriendDto>> GetRequestFriend(Guid userId);
        Task<List<User>> GetListByIdAsync(List<Guid> ids);
        Task<List<Guid>> GetCreatorsAsync(UserSimilarityRequest request);

    }
}