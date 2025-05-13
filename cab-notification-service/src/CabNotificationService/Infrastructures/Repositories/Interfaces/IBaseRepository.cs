using System.Linq.Expressions;

namespace CabNotificationService.Infrastructures.Repositories.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> CreateRangeAsync(IEnumerable<T> entities);
        Task<long> CountAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        Task<(IEnumerable<T>, string)> GetListPagingAsync(Expression<Func<T, bool>> predicate, int pageSize, byte[] pagingState = null); 
        Task<T> GetOneAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetLastOneAsync();
        Task<bool> UpdateAsync(T entity, Expression<Func<T, bool>> predicate, Expression<Func<T, T>> selector);
        Task<bool> DeleteAsync(T entity, Expression<Func<T, bool>> predicate);
        Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate);
    }
}