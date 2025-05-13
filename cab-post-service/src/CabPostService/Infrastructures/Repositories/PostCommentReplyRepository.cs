using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostCommentReplyRepository :
        BaseRepository<PostCommentReply>,
        IPostCommentReplyRepository
    {
        public PostCommentReplyRepository(ScyllaDbContext context)
            : base(context)
        {
        }
    }
}