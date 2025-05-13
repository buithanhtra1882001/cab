namespace CabPaymentService.Infrastructures.Repositories.Interfaces
{
    public interface IPostgresBaseRepository<TEntity, TId>
    {
        Task<TEntity> GetByIdAsync(TId id);
        Task<int> CreateAsync(TEntity entity);
        Task<int> UpdateAsync(TEntity entity);
        Task<int> DeleteAsync(TId id);
    }
}
