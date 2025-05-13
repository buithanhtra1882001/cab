using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;

namespace CabPostService.Handlers.PostCategory
{
    public partial class PostCategoryHandler :
        IQueryHandler<GetPostCategoryBySlugQuery, UserPostCategoryResponse>
    {
        public async Task<UserPostCategoryResponse> Handle(
            GetPostCategoryBySlugQuery request,
            CancellationToken cancellationToken)
        {
            var postCategoryRepository = _seviceProvider.GetRequiredService<IPostCategoryRepository>();

            var postCategory = await postCategoryRepository.GetBySlug(request.Slug);

            return _mapper.Map<UserPostCategoryResponse>(postCategory);
        }
    }
}

