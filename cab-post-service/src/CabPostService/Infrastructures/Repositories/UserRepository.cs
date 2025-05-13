using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories
{
    public class UserRepository :
        BaseRepository<User>,
        IUserRepository
    {
        public UserRepository(ScyllaDbContext context)
            : base(context)
        {
        }
    }
}
