using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Models.Commands;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        ICommandHandler<IncreaseViewPostCommand, long>
    {
        public async Task<long> Handle(
            IncreaseViewPostCommand request,
            CancellationToken cancellationToken)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            var postEntity = await db.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.PostId);

            if (postEntity is null)
            {
                _logger.LogWarning($"IncreaseViewPostCommand -> Cannot edit the post {request.PostId}, errors: not found the post");
                return 0;
            }

            postEntity.ViewCount += 1;
            postEntity.UpdatedAt = DateTime.UtcNow;

            db.Posts.Update(postEntity);
            await db.SaveChangesAsync();

            var viewResult = await db.Posts
                .AsNoTracking()
                .Where(x => x.Id == request.PostId)
                .Select(x => x.ViewCount)
                .FirstOrDefaultAsync();

            return viewResult;
        }
    }
}
