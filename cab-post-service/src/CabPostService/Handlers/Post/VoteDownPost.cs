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
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler : ICommandHandler<VoteDownPostCommand, bool>
    {
        public async Task<bool> Handle(VoteDownPostCommand request, CancellationToken cancellationToken)
        {
            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();
            var post = await postRepository.GetByIdAsync(request.PostId);
            if (post is null)
            {
                _logger.LogWarning($"Cannot edit the post {request.PostId}, errors: not found the post");
                throw new EntityNotFoundException("The post is not found");
            }

            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            var postVoteEntity = await db.PostVotes
               .FirstOrDefaultAsync(x => x.UserVoteId == request.UserId && x.PostId == post.Id);

            if (postVoteEntity is not null)
                throw new ApiValidationException("You has voted this post before");

            int currentPoint = post.Point;

            //post.Point = currentPoint + 1;
            post.VoteDownCount += 1;
            post.UpdatedAt = DateTime.UtcNow;
            await postRepository.UpdateAsync(post);

            postVoteEntity = new Models.Entities.PostVote
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PostId = request.PostId,
                UserVoteId = request.UserId,
                Type = PostConstant.VOTE_DOWN
            };
            db.PostVotes.Add(postVoteEntity);
            await db.SaveChangesAsync();

            var mediator = _seviceProvider.GetRequiredService<IMediator>();

            var existUser = await db.Users.FindAsync(request.UserId)
                ?? throw new EntityNotFoundException("The user is not found");

            var userBehaviorRequest = new UserBehaviorRequest
            {
                UserId = existUser.Id,
                PostId = post.Id,
                Type = UserActionType.Like
            };

            var actorInfo = new UserInfo(
                 userId: existUser.Id,
                 fullName: existUser.Fullname,
                 avatar: existUser.Avatar
               );

            var eventBus = _seviceProvider.GetRequiredService<IEventBus>();
            eventBus.Publish(new NotificationIntegrationEvent
                (new List<Guid> { post.UserId }, actorInfo, Guid.Parse(post.Id), NotificationConstants.UnLikePost));

            await mediator.Send(userBehaviorRequest);

            var cmdHashtag = new UpdateHashtagCommand()
            {
                Hashtags = post.Hashtags
            };
            await mediator.Publish(cmdHashtag);

            return true;
        }
    }
}
