using CabPostService.Constants;
using CabPostService.DomainCommands.Commands;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.SharePost
{
    public partial class SharePostHandler :
        ICommandHandler<CreateSharePostForUsersCommand, IList<Guid>>
    {
        public async Task<IList<Guid>> Handle(
            CreateSharePostForUsersCommand request,
            CancellationToken cancellationToken)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();
            var existingPost = await postRepository.GetByIdAsync(request.PostId);
            if (existingPost is null)
            {
                _logger.LogWarning($" Not found post by post = {request.PostId}");
                throw new ApiValidationException("The post is not found");
            }

            var existingUser = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (existingUser is null)
            {
                _logger.LogWarning($" Not found user by userId = {request.UserId}");
                throw new ApiValidationException("The user is not found");
            }

            var shareUserIds = await db.Users
                .Where(x => request.SharedUserIds.Contains(x.Id))
                .Select(x => x.Id).ToListAsync();

            if (shareUserIds.Count == 0)
            {
                _logger.LogError("No users specified for sharing the post");
                throw new ApiValidationException("No users specified for sharing the post");
            }

            var sharePosts = shareUserIds.Select(shareUserId => new Models.Entities.SharePost
            {
                Id = Guid.NewGuid(),
                UserId = existingUser.Id,
                PostId = existingPost.Id,
                SharedUserId = shareUserId,
                ShareLink = request.ShareLink,
                IsPublic = request.IsPublic,
            }).ToList();

            db.SharePosts.AddRange(sharePosts);
            await db.SaveChangesAsync();

            var mediator = _seviceProvider.GetRequiredService<IMediator>();
            var cmdHashtag = new UpdateHashtagCommand()
            {
                Hashtags = existingPost.Hashtags
            };
            await mediator.Publish(cmdHashtag);

            var sharedPosts = await db.SharePosts
                .Where(x => sharePosts.Select(x => x.Id).Contains(x.Id) && x.SharedUserId != null)
                .ToListAsync();

            var sharePostUserIds = sharedPosts.Select(x => x.SharedUserId.Value).ToList();

            return sharePostUserIds;
        }
    }
}
