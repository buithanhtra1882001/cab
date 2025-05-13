using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;

namespace CabPostService.Handlers.PostCategory
{
    public partial class PostCategoryHandler :
        IQueryHandler<GetPostCategoryByTypeQuery, IList<UserPostCategoryResponse>>
    {
        public async Task<IList<UserPostCategoryResponse>> Handle(
            GetPostCategoryByTypeQuery request,
            CancellationToken cancellationToken)
        {
            var postCategoryRepository = _seviceProvider.GetRequiredService<IPostCategoryRepository>();

            var postCategory = await postCategoryRepository.GetByType(request.Type);

            return _mapper.Map<IList<UserPostCategoryResponse>>(postCategory);
        }
    }
}

