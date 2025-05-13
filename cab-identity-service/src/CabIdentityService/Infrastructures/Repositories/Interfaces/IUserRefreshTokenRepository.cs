using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.Models.Entities;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Repositories.Interfaces
{
    public interface IUserRefreshTokenRepository : IBaseRepository<UserRefreshToken>
    {
        Task<UserRefreshToken> GetByTokenValueAsync(string token);
        Task<bool> CleanUpExpiredTokensAsync(int subjectId);
    }
}
