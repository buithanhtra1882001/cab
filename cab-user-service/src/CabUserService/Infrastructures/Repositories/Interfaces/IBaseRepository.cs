using System.Linq.Expressions;

namespace CabUserService.Infrastructures.Repositories.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<(IEnumerable<T>, string)> GetListPagingAsync(Expression<Func<T, bool>> predicate, int pageSize, byte[] pagingState);
        Task<(IEnumerable<T>, (string, string))> GetListByConditionOrPagingAsync(Expression<Func<T, bool>> predicateFirst, Expression<Func<T, bool>> predicateLast, int pageSize, byte[] pagingStateFirst, byte[] pagingStateLast);
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetOneAsync(Expression<Func<T, bool>> predicate);
        Task<bool> UpdateAsync(T entity, Expression<Func<T, bool>> predicate, Expression<Func<T, T>> selector);
        Task<bool> DeleteAsync(T entity, Expression<Func<T, bool>> predicate);
    }
}