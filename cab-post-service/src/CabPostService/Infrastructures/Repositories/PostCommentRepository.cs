using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostCommentRepository :
        BaseRepository<PostComment>,
        IPostCommentRepository
    {
        public PostCommentRepository(ScyllaDbContext context)
            : base(context)
        {
        }
    }
}