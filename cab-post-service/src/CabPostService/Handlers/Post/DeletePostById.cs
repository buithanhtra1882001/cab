using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using LazyCache;

namespace CabPostService.Handlers.Post
{
    public partial class PostHandler :
        ICommandHandler<DeletePostCommand, bool>
    {
        public async Task<bool> Handle(
            DeletePostCommand request,
            CancellationToken cancellationToken)
        {
            var postRepository = _seviceProvider.GetRequiredService<IPostRepository>();

            var post = await ValidationDeleteAndReturnPostAsync(request, postRepository);

            if (!request.IsSoftDelete)
            {
                await postRepository.DeleteAsync(post.Id);

                var postCommentPostIdMaterializedViewRepository = _seviceProvider.GetRequiredService<IPostCommentPostIdMaterializedViewRepository>();

                var commentIds = (await postCommentPostIdMaterializedViewRepository
                    .GetListAsync(item => item.PostId == post.Id))
                    .Select(item => item.Id)
                    .ToList();

                var postCommentRepository = _seviceProvider.GetRequiredService<IPostCommentRepository>();

                await postCommentRepository.DeleteAsync(item => commentIds.Contains(item.Id));
            }
            else
            {
                post.IsSoftDeleted = true;
                await postRepository.UpdateAsync(post);
            }

            DeletePostFromCache(request.PostId);

            return true;
        }

        private void DeletePostFromCache(string id)
        {
            var cache = _seviceProvider.GetRequiredService<IAppCache>();

            var posts = cache.Get<List<UserPostResponse>>(CacheKeyConstant.POSTS);
            if (posts is null)
                return;

            var post = posts.FirstOrDefault(item => item.Id == id);
            if (post is not null)
                posts.Remove(post);
        }

        private async Task<Models.Entities.Post> ValidationDeleteAndReturnPostAsync(
            DeletePostCommand request,
            IPostRepository postRepository)
        {
            var post = await postRepository.GetByIdAsync(request.PostId);
            if (post is null)
            {
                _logger.LogWarning($"Cannot delete the post {request.PostId}, errors: not found the post");
                throw new ApiValidationException("The post is not found");
            }

            if (post.UserId != request.UserId)
            {
                _logger.LogError($"Cannot delete the post {request.PostId}, errors: the post not belong to you");
                throw new ApiValidationException("The post not belong to you");
            }

            return post;
        }
    }
}