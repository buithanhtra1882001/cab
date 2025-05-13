using CabUserService.Models.Entities;

namespace CabUserService.Infrastructures.Repositories.Interfaces
{
    public interface ICategoryRepository:IPostgresBaseRepository<Category>
    {
         Task<List<Category>> GetAllCategoriesAsync();
    }
}
