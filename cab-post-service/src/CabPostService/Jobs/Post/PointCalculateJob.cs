using CabPostService.Constants;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Jobs.Base;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using MediatR;

namespace CabPostService.Jobs.Post
{
    public class PointCalculateJob : BaseJob<PointCalculateJob>
    {
        public PointCalculateJob(
            IServiceProvider serviceProvider,
            ILogger<PointCalculateJob> logger)
            : base(serviceProvider, logger)
        {
            _logger = logger;
            _logger.LogInformation(" -- Create PointCalculateJob successfully ! --");
        }

        public async Task CalculatePointAsync()
        {
            _logger.LogInformation(" -- Start Run CalculatePoint JOB ! --");
            var postRepository = _serviceProvider.GetRequiredService<IPostRepository>();

            var allPosts = await postRepository.GetAllAsync(new GetAllPostFilter());
            if (allPosts is null || allPosts.Count == 0)
            {
                _logger.LogInformation(" -- Complete CalculatePoint JOB ! --");
                return;
            }

            var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            var mediator = _serviceProvider.GetRequiredService<IMediator>();

            _logger.LogInformation("Post size:" + allPosts.Count);

            foreach (var post in allPosts)
            {
                var result = await mediator.Send(new PostFinalScoreCalculateCommand
                {
                    Post = post,
                    UserId = post.UserId,
                });

                _logger.LogInformation($"Final score of Post ID: {post.Id}, Title: {post.Title} is : " + result);

                var postIsValidToUpdate = !post.IsSoftDeleted &&
                                          post.IsChecked &&
                                          post.Status is PostConstant.ACTIVE;

                if (postIsValidToUpdate)
                {
                    post.Point = (int)result;
                    await postRepository.UpdateAsync(post);
                }
            }

            _logger.LogInformation(" -- Complete CalculatePoint JOB ! --");
        }
    }
}
