using CAB.BuildingBlocks.EventBus.Abstractions;
using CabPostService.Constants;
using CabPostService.DomainCommands.Commands;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.IntegrationEvents.Events;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace CabPostService.Handlers.Comment
{
    public partial class CommentHandler : ICommandHandler<CommentCommand, Guid>
    {
        public async Task<Guid> Handle(
            CommentCommand request,
            CancellationToken cancellationToken)
        {
            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();


            var comment = _mapper.Map<PostComment>(request);
            comment.Id = Guid.NewGuid();
            comment.Status = PostConstant.ACTIVE;

            var post = await postRepository.GetByIdAsync(comment.PostId);
            if (post is null)
            {
                _logger.LogWarning($"Not found post with postId={comment.PostId}");
                throw new ApiValidationException("The post is not found");
            }

            var postCommentRepository = _seviceProvider.GetRequiredService<IPostCommentRepository>();
            await postCommentRepository.CreateAsync(comment);
            await postRepository.UpdateCommentCount(comment.PostId);

            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            var existUser = await db.Users.FindAsync(request.UserId)
                ?? throw new EntityNotFoundException("The user is not found");

            var actorInfo = new UserInfo(
                  userId: existUser.Id,
                  fullName: existUser.Fullname,
                  avatar: existUser.Avatar
                );

            var eventBus = _seviceProvider.GetRequiredService<IEventBus>();
            eventBus.Publish(new NotificationIntegrationEvent
                (new List<Guid> { post.UserId }, actorInfo, Guid.Parse(post.Id), NotificationConstants.Comment));

            var postUserRepository = _seviceProvider.GetRequiredService<IPostUserRepository>();
            if (postRepository is not null && !await postUserRepository.CheckExist(comment.PostId, existUser.Id))
            {
                var resultInsert = await postUserRepository.CreateAsync(new PostUsers
                {
                    PostId = comment.PostId,
                    UserId = existUser.Id,
                    IsShowPost = false
                });
                _logger.LogInformation($"Save User" + request.UserId + " Follow Post " + post.Id + " : " + resultInsert);
            }
            else
            {
                _logger.LogInformation($" User" + request.UserId + "Already Follow Post " + post.Id);
            }

            UpdatePostFromCache(comment.PostId);

            var mediator = _seviceProvider.GetRequiredService<IMediator>();

            var cmdHashtag = new UpdateHashtagCommand()
            {
                Hashtags = post.Hashtags
            };
            await mediator.Publish(cmdHashtag);

            var userBehaviorRequest = new UserBehaviorRequest
            {
                UserId = request.UserId,
                PostId = post.Id,
                Type = UserActionType.Comment
            };

            await mediator.Send(userBehaviorRequest);

            await AddCommentToCache(comment);

            return comment.Id;
        }

        private async Task AddCommentToCache(PostComment comment)
        {
            var cache = _seviceProvider.GetRequiredService<IAppCache>();
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            var newComment = _mapper.Map<UserCommentResponse>(comment);
            var comments = cache.Get<List<UserCommentResponse>>($"{CacheKeyConstant.POST_COMMENTS}_{comment.PostId}");
            var user = await db.Users.FirstOrDefaultAsync(item => item.Id == newComment.UserId);
            newComment.UserFullName = user is null ? string.Empty : user.Fullname;
            newComment.Avatar = user is null ? string.Empty : user.Avatar;

            comments?.Add(newComment);
        }

        private void UpdatePostFromCache(string postId)
        {
            var cache = _seviceProvider.GetRequiredService<IAppCache>();

            var posts = cache.Get<List<UserPostResponse>>(CacheKeyConstant.POSTS);
            if (posts is null)
                return;

            var post = posts.FirstOrDefault(item => item.Id == postId);
            if (post is not null)
                post.CommentsCount += 1;
        }
    }
}
