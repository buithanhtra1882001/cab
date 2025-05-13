using AutoMapper;
using CabPostService.Constants;
using CabPostService.Handlers.Base;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler : BaseHandler<PostHandler>,
        ICommandHandler<PostFinalScoreCalculateCommand, decimal>
    {
        private readonly IConfiguration _configuration;
        public PostHandler(
            IServiceProvider serviceProvider,
            ILogger<PostHandler> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IConfiguration configuration) :
            base(serviceProvider, logger, httpContextAccessor, mapper)
        {
            _configuration = configuration;
        }

        private decimal PostEngagementScoreCalculate(Models.Entities.Post post)
        {
            if (post is null)
            {
                _logger.LogWarning("PostEngagementScoreCalculate error: post is not found");
                return 0;
            }

            decimal weightedUpVoteScore = WeightConstants.UpVoteWeight * post.VoteUpCount;
            decimal weightedDownVoteScore = WeightConstants.DownVoteWeight * post.VoteDownCount;
            decimal weightedCommentScore = WeightConstants.TotalCommentsWeight * post.CommentsCount;
            decimal weightedViewScore = WeightConstants.TotalViewsWeight * post.ViewCount;

            decimal pointTotalVoteUp = post.VoteUpCount * 2;
            decimal pointTotalVoteDown = post.VoteUpCount * 1;

            return weightedUpVoteScore
                   + weightedDownVoteScore
                   + weightedCommentScore
                   + weightedViewScore
                   + pointTotalVoteUp
                   + pointTotalVoteDown;
        }

        private async Task<double> PostVideoRewardPenaltyScoreCalculateAsync(string postId)
        {
            try
            {
                if (string.IsNullOrEmpty(postId))
                    return 0;

                var postVideoRepository = _seviceProvider.GetRequiredService<IPostVideoRepository>();
                var postVideos = await postVideoRepository.GetByPostIdsAsync(postId);
                if (postVideos is null || postVideos.Count == 0)
                {
                    _logger.LogWarning($"PostVideoRewardPenaltyScoreCalculate postVideos is not found");
                    return 0;
                }

                int weightOfRewardPenalty = RewardPenaltyConstants.WeightOfRewardPenalty;
                int steepness = RewardPenaltyConstants.Steepness;
                double threshold = RewardPenaltyConstants.Threshold;
                double euler = RewardPenaltyConstants.EulerConst;

                double result = 0;

                foreach (var postVideo in postVideos)
                {
                    var pow = Math.Pow(euler, steepness * (postVideo.AvgViewLength - threshold));
                    result += weightOfRewardPenalty * (1 / (1 + pow) - 0.5);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PostVideoRewardPenaltyScoreCalculateAsync errors: {ex.Message}");
                return 0;
            }
        }

        public async Task<decimal> Handle(PostFinalScoreCalculateCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                _logger.LogWarning("PostFinalScoreCalculateAsync error: request is null");
                return 0;
            }

            if (request.Post is null)
            {
                _logger.LogWarning("PostFinalScoreCalculateAsync error: post is not found");
                return 0;
            }

            decimal rewardPenaltyScore = (decimal)await PostVideoRewardPenaltyScoreCalculateAsync(request.Post.Id);

            var postCategoryRepository = _seviceProvider.GetRequiredService<IPostCategoryRepository>();
            var category = await postCategoryRepository.GetByIdAsync(request.Post.CategoryId);

            var categoryScore = category is null ? 0 : category.Score;

            var engagementScore = PostEngagementScoreCalculate(request.Post);
            var categoryScoreOfPost = WeightConstants.CategoryScoreWeight * categoryScore;
            var posterScoreOfPost = WeightConstants.PosterScoreWeight * request.Post.PosterScore;
            var adminBootScore = request.Post.AdminBoost ? WeightConstants.AdminBoostWeight : 0;
            var userPreferenceForCategoryOfPostScore = 1;

            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            var isUserFollowThePost = db.PostUsers.AsNoTracking().Any(x => x.UserId == request.UserId);
            var userFollowPostScore = isUserFollowThePost ? 1 : 0; // 1 if user follow that post else 0

            return (categoryScoreOfPost + posterScoreOfPost + engagementScore + adminBootScore + rewardPenaltyScore)
                * (userPreferenceForCategoryOfPostScore + userFollowPostScore);
        }
    }
}