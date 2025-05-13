using CabUserService.Models.Entities.Base;

namespace CabUserService.Infrastructures.Repositories.Interfaces
{
    public interface IPostgresBaseRepository<T> where T : BaseEntity
    {
        Task<T> GetByIdAsync<IdType>(IdType id);
        Task<int> CreateAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task<int> HardDeleteAsync<IdType>(IdType id);
    }
}
