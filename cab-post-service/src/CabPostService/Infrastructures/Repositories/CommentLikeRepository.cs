using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories
{
    public class CommentLikeRepository :
         BaseRepository<CommentLike>,
        ICommentLikeRepository
    {
        public CommentLikeRepository(ScyllaDbContext context) : base(context)
        {
        }
    }
}
