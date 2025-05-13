using System.Linq.Expressions;
using CabMediaService.Infrastructures.DbContexts;
using CabMediaService.Infrastructures.Repositories.Interfaces;
using CabMediaService.Models.Entities.Base;
using Cassandra.Data.Linq;

namespace CabMediaService.Infrastructures.Repositories.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        #region props

        //private readonly CassandraDbContext _context;
        private readonly ScyllaDbContext _context;
        private Table<T> _entitiesDbSet { get; set; }

        #endregion props

        #region ctor

        //public BaseRepository(CassandraDbContext context)
        //{
        //    _context = context;
        //}

        public BaseRepository(ScyllaDbContext context)
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
            var result = await Entities.Where(predicate).AllowFiltering().ExecuteAsync();
            return result;
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            ValidateAndThrow(entity);
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

        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            await Entities.Where(predicate)
                .Delete().ExecuteAsync();
            return true;
        }

        public virtual Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            var result = Entities.Any(predicate);
            return Task.FromResult(result);
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