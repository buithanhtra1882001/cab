using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories
{
    public class ImageCommentRepository :
        BaseRepository<ImageComment>,
        IImageCommentRepository
    {
        public ImageCommentRepository(ScyllaDbContext context)
            : base(context)
        {
        }
    }
}
