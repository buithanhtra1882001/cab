using CabUserService.Models.Dtos;

namespace CabUserService.Services.Interfaces
{
    public interface ICategoryService
    {
         Task<List<CategoryResponse>> GetAllCategoriesAsync();
    }
}
