using System.Linq.Expressions;

namespace CabPaymentService.Infrastructures.Repositories.Interfaces
{
    public interface ICassandraBaseRepository<T>
    {
        Task<T> CreateAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        Task<T> GetOneAsync(Expression<Func<T, bool>> predicate);
        Task<bool> UpdateAsync(T entity, Expression<Func<T, bool>> predicate, Expression<Func<T, T>> selector);
        Task<bool> DeleteAsync(T entity, Expression<Func<T, bool>> predicate);
    }
}
