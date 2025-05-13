using CabPostService.Constants;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Jobs.Base;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Jobs.Post
{
    public class TrendingPointCalculateJob : BaseJob<TrendingPointCalculateJob>
    {
        public TrendingPointCalculateJob(
            IServiceProvider serviceProvider,
            ILogger<TrendingPointCalculateJob> logger)
            : base(serviceProvider, logger)
        {
            _logger = logger;
            _logger.LogInformation("-- Create TrendingPointCalculateJob successfully ! --");
        }

        public async Task CalculateTrendingPointAsync()
        {
            _logger.LogInformation(" -- Start Run TrendingPointCalculateJob JOB ! --");
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
            var postCommentPostIdMaterializedViewRepository = _serviceProvider
                .GetRequiredService<IPostCommentPostIdMaterializedViewRepository>();

            var cutoffDateTime = DateTime.UtcNow.AddHours(-24);
            var resentPosts = await db.Posts
                .AsNoTracking()
                .Where(x => !x.IsSoftDeleted
                            && x.IsChecked
                            && x.Status == PostConstant.ACTIVE
                            && x.CreatedAt >= cutoffDateTime)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            if (!resentPosts.Any())
            {
                _logger.LogInformation(" -- Complete TrendingPointCalculateJob JOB ! --");
                return;
            }

            var postIds = resentPosts.Select(x => x.Id).ToList();
            var shares = await db.SharePosts
                .AsNoTracking()
                .Where(x => x.CreatedAt >= cutoffDateTime
                && postIds.Contains(x.PostId))
                .ToListAsync();

            var comments = await postCommentPostIdMaterializedViewRepository
                .GetListAsync(x => x.CreatedAt >= cutoffDateTime
                && postIds.Contains(x.PostId));

            int maxLikes = resentPosts.Any() ? resentPosts.Max(x => x.VoteUpCount) : 1;
            int maxShares = shares.Any() ? shares.GroupBy(x => new { x.PostId }).Max(g => g.Count()) : 1;

            var postsToUpdate = new List<Models.Entities.Post>();

            foreach (var post in resentPosts)
            {
                var shareCount = shares.Count(x => x.PostId == post.Id);
                var commentCount = comments.Count(x => x.PostId == post.Id);

                var timeScore = resentPosts.IndexOf(post) < 10 ? 5 : 0;
                var shareScore = (int)((shareCount / (double)maxShares) * 2);
                var likeScore = (int)((post.VoteUpCount / (double)maxLikes) * 2);
                var commentScore = (int)Math.Min((commentCount * 0.1), 1);

                post.TrendingPoint = timeScore + shareScore + likeScore + commentScore;
                postsToUpdate.Add(post);

                _logger.LogInformation($"Final toptrending score of Post ID: {post.Id}");
            }

            db.Posts.UpdateRange(postsToUpdate);
            await db.SaveChangesAsync();

            _logger.LogInformation(" -- Complete TrendingPointCalculateJob JOB ! --");
        }
    }
}
