using CabPostService.Constants;
using CabPostService.DomainCommands.Commands;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.SharePost
{
    public partial class SharePostHandler :
    ICommandHandler<SharePostCommand, SharePostResponse>
    {
        public async Task<SharePostResponse> Handle(
            SharePostCommand request,
            CancellationToken cancellationToken)
        {
            await ValidationCreatePostShareAsync(request);

            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();
            var existingPost = await postRepository.GetByIdAsync(request.PostId);
            if (existingPost is null)
            {
                _logger.LogWarning($" Not found post by post = {request.PostId}");
                throw new ApiValidationException("The post is not found");
            }

            var postShare = _mapper.Map<Models.Entities.SharePost>(request);
            postShare.Id = Guid.NewGuid();
            await SharePostRepository.CreateAsync(postShare);

            var postShareResponse = _mapper.Map<SharePostResponse>(postShare);

            await AddPostShareToCache(postShareResponse);

            var mediator = _seviceProvider.GetRequiredService<IMediator>();
            var cmdHashtag = new UpdateHashtagCommand()
            {
                Hashtags = existingPost.Hashtags
            };
            await mediator.Publish(cmdHashtag);

            return postShareResponse;
        }

        private Task AddPostShareToCache(SharePostResponse sharePostResponse)
        {

            var postShareResponses =
              AppCache.Get<List<SharePostResponse>>(CacheKeyConstant.POST_SHARES);

            if (postShareResponses is null)
            {
                postShareResponses = new List<SharePostResponse>();
                AppCache.Add(CacheKeyConstant.POST_SHARES, postShareResponses);
            }

            var postShareIndexInCache = postShareResponses.FindIndex(item => item.Id == sharePostResponse.Id);

            if (postShareIndexInCache > -1)
                postShareResponses[postShareIndexInCache] = sharePostResponse;
            else
                postShareResponses.Add(sharePostResponse);

            return Task.CompletedTask;
        }

        private async Task ValidationCreatePostShareAsync(SharePostCommand request)
        {
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            var user = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
            if (user is null)
            {
                _logger.LogWarning($"Cannot create the post share with userId={request.UserId} not found");
                throw new ApiValidationException("The user is not found");
            }

            var postCategory = await PostRepository.GetByIdAsync(request.PostId);
            if (postCategory is null)
            {
                _logger.LogWarning($"Cannot create the post share with postId={request.PostId} not found");
                throw new ApiValidationException("The post is not found");
            }
        }
    }
}
