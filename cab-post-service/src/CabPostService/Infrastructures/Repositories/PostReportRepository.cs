using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostReportRepository :
        BaseRepository<PostReport>,
        IPostReportRepository
    {
        public PostReportRepository(ScyllaDbContext context)
            : base(context)
        {
        }
    }
}
