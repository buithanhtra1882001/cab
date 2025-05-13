using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostCommentPostIdMaterializedViewRepository :
        BaseRepository<PostCommentPostIdMaterializedView>,
        IPostCommentPostIdMaterializedViewRepository
    {
        private readonly Cassandra.ISession _session;
        public PostCommentPostIdMaterializedViewRepository(ScyllaDbContext context)
            : base(context)
        {
            _session = context._session;
        }

       
    }
}
