using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Queries;

namespace CabPostService.Handlers.PostCategory
{
    public partial class PostCategoryHandler :
        IQueryHandler<GetPostCategoryQuery, IList<UserPostCategoryResponse>>
    {
        public async Task<IList<UserPostCategoryResponse>> Handle(
            GetPostCategoryQuery request,
            CancellationToken cancellationToken)
        {
            var postCategoryRepository = _seviceProvider.GetRequiredService<IPostCategoryRepository>();

            var postCategories = await postCategoryRepository.GetAllAsync();

            return _mapper.Map<List<UserPostCategoryResponse>>(postCategories);
        }
    }
}
