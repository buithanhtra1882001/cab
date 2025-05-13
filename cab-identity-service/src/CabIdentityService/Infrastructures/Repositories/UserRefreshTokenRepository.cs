using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.Infrastructures.DbContexts;
using WCABNetwork.Cab.IdentityService.Infrastructures.Repositories.Base;
using WCABNetwork.Cab.IdentityService.Infrastructures.Repositories.Interfaces;
using WCABNetwork.Cab.IdentityService.Models.Entities;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Repositories
{
    public class UserRefreshTokenRepository : BaseRepository<UserRefreshToken>, IUserRefreshTokenRepository
    {
        public UserRefreshTokenRepository(IdentityCoreDbContext identityCoreDbContext)
            : base (identityCoreDbContext)
        {

        }

        public async Task<bool> CleanUpExpiredTokensAsync(int subjectId)
        {
            var result = await Entities.Where(x => x.SubjectId == subjectId && x.Expiration < DateTime.UtcNow)
                .DeleteFromQueryAsync();
            return result > 0;
        }

        public async Task<UserRefreshToken> GetByTokenValueAsync(string token)
        {
            return await GetNoTrackingEntities().FirstOrDefaultAsync(x => x.TokenValue == token);
        }
    }
}
