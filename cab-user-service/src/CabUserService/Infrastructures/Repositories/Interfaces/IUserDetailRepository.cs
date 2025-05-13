using CabUserService.Models.Entities;

namespace CabUserService.Infrastructures.Repositories.Interfaces
{
    public interface IUserDetailRepository : IPostgresBaseRepository<UserDetail>
    {
        Task<int> UpdateFollowerAsync(Guid userId, string follower);
        Task<int> UpdateFollowingAsync(Guid userId, string following);
        Task<UserDetail> GetUserDetailByUserIdAsync(Guid userId);
        Task<List<UserDetail>> GetListDetailByIdAsync(List<Guid> ids);


    }
}