using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostDonateRepository :
        BaseRepository<PostDonate>,
        IPostDonateRepository
    {
        public PostDonateRepository(
            ScyllaDbContext context)
            : base(context)
        {
        }
    }
}
