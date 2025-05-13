using CabPaymentService.Infrastructures.DbContexts;
using CabPaymentService.Infrastructures.Repositories.Interfaces;
using CabPaymentService.Model.Entities.Base;
using Npgsql;
using System.Data;
using CabPaymentService.Model.Entities;

namespace CabPaymentService.Infrastructures.Repositories.Base
{
    public class PostgresBaseRepository<TEntity, TId> :
        IPostgresBaseRepository<TEntity, TId> where TEntity : Entity
    {
        private readonly IConfiguration _configuration;
        private readonly PostgresDbContext _context;

        protected PostgresBaseRepository(PostgresDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            var result = await _context.SaveChangesAsync();
            return result;
        }

        public async Task<int> DeleteAsync(TId id)
        {
            var result = 0;
            var dbSet = _context.Set<TEntity>();
            var record = await dbSet.FindAsync(id);
            if (record != null)
            {
                dbSet.Remove(record);
                result = await _context.SaveChangesAsync();
            }
            return result;
        }

        public async Task<TEntity> GetByIdAsync(TId id)
        {
            var result = await _context.Set<TEntity>().FindAsync(id);
            return result;
        }

        public async Task<int> UpdateAsync(TEntity entity)
        {
            var dbSet = _context.Set<TEntity>();
            dbSet.Update(entity);
            var result = await _context.SaveChangesAsync();
            return result;
        }
    }
}

