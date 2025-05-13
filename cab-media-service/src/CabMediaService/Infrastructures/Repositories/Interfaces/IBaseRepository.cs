using System.Linq.Expressions;

namespace CabMediaService.Infrastructures.Repositories.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetOneAsync(Expression<Func<T, bool>> predicate);
        Task<bool> UpdateAsync(T entity, Expression<Func<T, bool>> predicate, Expression<Func<T, T>> selector);
        Task<bool> DeleteAsync(T entity, Expression<Func<T, bool>> predicate);
        Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}