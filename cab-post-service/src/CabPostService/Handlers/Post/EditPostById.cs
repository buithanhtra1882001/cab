using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        ICommandHandler<EditPostCommand, bool>
    {
        public async Task<bool> Handle(
            EditPostCommand request,
            CancellationToken cancellationToken)
        {
            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();

            var post = await ValidationEditAndReturnPostAsync(request, postRepository);

            var postEntity = _mapper.Map(request, post);

            await postRepository.UpdateAsync(postEntity);

            return true;
        }

        private async Task<Models.Entities.Post> ValidationEditAndReturnPostAsync(
            EditPostCommand request,
            IPostRepository postRepository)
        {
            var post = await postRepository.GetByIdAsync(request.PostId);
            if (post is null)
            {
                _logger.LogWarning($"Cannot edit the post {request.PostId}, errors: not found the post");
                throw new ApiValidationException("The post is not found");
            }

            if (post.UserId != request.UserId)
            {
                _logger.LogError($"Cannot edit the post {request.PostId}, errors: the post not belong to you");
                throw new ApiValidationException("The post not belong to you");
            }

            return post;
        }
    }
}
