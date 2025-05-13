using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories.Interfaces
{
    public interface IPostCategoryRepository : IPostgresBaseRepository<PostCategory>
    {
        Task<List<PostCategory>> GetAllAsync();
        Task<List<PostCategory>> GetAllAsync(GetAllPostCategoryFilter filter);
        Task<PostCategory> GetBySlug(string slug);
        Task<long> GetTotalAsync(GetAllPostCategoryFilter filter);
        void InsertCategories(List<PostCategory> postCategories);
        void SeedDataCategoryAndCategoryType();
        Task<int> CountPostCategory();
        Task<List<PostCategory>> GetByType(int type);
    }
}