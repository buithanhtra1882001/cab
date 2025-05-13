using CabPaymentService.Infrastructures.DbContexts;
using CabPaymentService.Infrastructures.Repositories.Interfaces;
using CabPaymentService.Model.Entities.Base;
using Cassandra.Data.Linq;
using System.Linq.Expressions;

namespace CabPaymentService.Infrastructures.Repositories.Base
{
    public class CassandraBaseRepository<T> : ICassandraBaseRepository<T> where T : BaseEntity
    {
        #region props

        private readonly CassandraDbContext _context;
        private Table<T> _entitiesDbSet { get; set; }

        #endregion props

        #region ctor

        public CassandraBaseRepository(CassandraDbContext context)
        {
            _context = context;
        }

        #endregion ctor

        #region public

        public virtual async Task<T> GetOneAsync(Expression<Func<T, bool>> predicate)
        {
            var result = await Entities.FirstOrDefault(predicate).ExecuteAsync();
            return result;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var result = await Entities.ExecuteAsync();
            return result;
        }

        public virtual async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            var result = await Entities.Where(predicate).ExecuteAsync();
            return result;
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            ValidateAndThrow(entity);
            AddDefaultValue(ref entity);
            await Entities.Insert(entity).ExecuteAsync();
            return entity;
        }

        public virtual async Task<bool> UpdateAsync(T entity, Expression<Func<T, bool>> predicate, Expression<Func<T, T>> selector)
        {
            ValidateAndThrow(entity);
            await Entities.Where(predicate)
                .Select(selector)
                .Update().ExecuteAsync();
            return true;
        }

        public virtual async Task<bool> DeleteAsync(T entity, Expression<Func<T, bool>> predicate)
        {
            ValidateAndThrow(entity);
            await Entities.Where(predicate)
                .Delete().ExecuteAsync();
            return true;
        }

        #endregion public

        #region private

        private static void ValidateAndThrow(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
        }

        private static void AddDefaultValue(ref T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = entity.CreatedAt;
        }

        protected Table<T> Entities
        {
            get
            {
                if (_entitiesDbSet == null)
                {
                    _entitiesDbSet = _context.GetTable<T>();
                }
                return _entitiesDbSet;
            }
        }
        #endregion private
    }
}
