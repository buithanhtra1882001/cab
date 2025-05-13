using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Dtos;
using LazyCache;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Handlers.PostCategory
{
    public partial class PostCategoryHandler :
        ICommandHandler<CreatePostCategoryCommand, Guid>
    {
        public async Task<Guid> Handle(
            CreatePostCategoryCommand request,
            CancellationToken cancellationToken)
        {
            var postCategory = _mapper.Map<Models.Entities.PostCategory>(request);
            postCategory.Id = Guid.NewGuid();
            postCategory.Status = PostConstant.ACTIVE;

            var postCategoryRepository = _seviceProvider.GetRequiredService<IPostCategoryRepository>();

            await postCategoryRepository.CreateAsync(postCategory);

            await AddPostCategoryToCache(postCategory);
            return postCategory.Id;
        }

        private async Task AddPostCategoryToCache(Models.Entities.PostCategory postCategory)
        {
            var cache = _seviceProvider.GetRequiredService<IAppCache>();
            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();

            var userPostCategory = _mapper.Map<UserPostCategoryResponse>(postCategory);
            var postCategories = cache.Get<List<UserPostCategoryResponse>>(CacheKeyConstant.POSTCATEGORYS);

            if (postCategories is not null)
            {
                var user = await db.Users.FirstOrDefaultAsync(item => item.Id == userPostCategory.Id);
                postCategories.Add(userPostCategory);
            }
        }
    }
}