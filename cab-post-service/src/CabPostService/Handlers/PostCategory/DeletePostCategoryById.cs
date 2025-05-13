using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using LazyCache;

namespace CabPostService.Handlers.PostCategory
{
    public partial class PostCategoryHandler :
        ICommandHandler<DeletePostCategoryCommand, bool>
    {
        public async Task<bool> Handle(
            DeletePostCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var postCategoryRepository = _seviceProvider.GetRequiredService<IPostCategoryRepository>();

            var postCategory = await postCategoryRepository.GetByIdAsync(request.PostCategoryId);
            if (postCategory is null)
            {
                _logger.LogWarning($"Cannot delete the post category {request.PostCategoryId}, errors: not found the post");
                throw new ApiValidationException("The post category is not found");
            }

            if (!request.IsSoftDelete)
            {
                await postCategoryRepository.DeleteAsync(postCategory.Id);
            }
            else
            {
                postCategory.IsSoftDeleted = true;
                await postCategoryRepository.UpdateAsync(postCategory);
            }

            DeletePostCategoryFromCache(request.PostCategoryId);

            return true;
        }

        private void DeletePostCategoryFromCache(Guid id)
        {
            var cache = _seviceProvider.GetRequiredService<IAppCache>();

            var postCategories = cache.Get<List<UserPostCategoryResponse>>(CacheKeyConstant.POSTCATEGORYS);
            if (postCategories is null)
                return;

            var post = postCategories.FirstOrDefault(item => item.Id == id);
            if (post is not null)
                postCategories.Remove(post);
        }
    }
}