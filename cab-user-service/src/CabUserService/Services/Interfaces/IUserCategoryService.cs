using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;

namespace CabUserService.Services.Interfaces
{
    public interface IUserCategoryService
    {
        Task<string> FollowCategoriesAsync(Guid userId, List<Guid> categoryIds);
        Task<List<CategoryResponse>> GetUserFollowedCategoriesAsync(Guid userId);
    }
}
