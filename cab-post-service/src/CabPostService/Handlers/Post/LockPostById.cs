using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        ICommandHandler<LockPostCommand, bool>
    {

        public async Task<bool> Handle(
            LockPostCommand request,
            CancellationToken cancellationToken)
        {
            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();

            var post = await postRepository.GetByIdAsync(request.PostId);
            if (post is null)
            {
                _logger.LogWarning($"Cannot lock the post {request.PostId}, errors: not found the post");
                throw new ApiValidationException("The post is not found");
            }

            if (post.Status is not PostConstant.INACTIVE)
            {
                post.Status = PostConstant.INACTIVE;
                await postRepository.UpdateAsync(post);
            }

            return true;
        }
    }
}