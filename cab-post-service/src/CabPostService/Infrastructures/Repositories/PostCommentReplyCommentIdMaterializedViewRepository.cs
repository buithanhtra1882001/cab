using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostCommentReplyCommentIdMaterializedViewRepository :
        BaseRepository<PostCommentReplyCommentIdMaterializedView>,
        IPostCommentReplyCommentIdMaterializedViewRepository
    {
        public PostCommentReplyCommentIdMaterializedViewRepository(ScyllaDbContext context)
            : base(context)
        {
        }
    }
}
