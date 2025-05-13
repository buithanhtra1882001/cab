using CabUserService.Infrastructures.DbContexts;
using CabUserService.Infrastructures.Repositories.Base;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Entities;

namespace CabUserService.Infrastructures.Repositories
{
    public class UserFriendRepository : BaseRepository<UserFriend>, IUserFriendRepository
    {
        public UserFriendRepository(ScyllaDbContext context)
            : base(context)
        {
        }
    }
}
