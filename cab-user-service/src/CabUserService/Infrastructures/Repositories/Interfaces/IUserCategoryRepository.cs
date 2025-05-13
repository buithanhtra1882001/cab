using CabUserService.Models.Entities;

namespace CabUserService.Infrastructures.Repositories.Interfaces
{
    public interface IUserCategoryRepository : IPostgresBaseRepository<UserCategory>
    {
        Task<UserCategory> GetUserCategoryByIdAsync(Guid userId, Guid categoryId);
        Task<List<Category>> GetUserFollowedCategoriesAsync(Guid userId);
        Task<int> AddRangeUserCategoryAsync(List<UserCategory> categories);
    }
}
