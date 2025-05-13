using CabUserService.Infrastructures.DbContexts;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Entities.Base;
using Cassandra.Data.Linq;
using System.Linq.Expressions;

namespace CabUserService.Infrastructures.Repositories.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        #region props

        //private readonly CassandraDbContext _context;
        private readonly ScyllaDbContext _context;
        private Table<T> _entitiesDbSet { get; set; }

        #endregion props

        #region ctor

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

        public virtual async Task<(IEnumerable<T>, string)> GetListPagingAsync(Expression<Func<T, bool>> predicate, int pageSize, byte[] pagingState)
        {
            var result = await Entities
                .Where(predicate)
                .OrderByDescending(x => x.CreatedAt)
                .SetPageSize(pageSize)
                .SetPagingState(pagingState)
                .ExecutePagedAsync();

            string pagingStateBase64String = (result.Count() != 0 && pagingState != null) ? Convert.ToBase64String(result.PagingState) : string.Empty;

            return (result, pagingStateBase64String);
        }

        public virtual async Task<(IEnumerable<T>, (string, string))> GetListByConditionOrPagingAsync(Expression<Func<T, bool>> predicateFirst, Expression<Func<T, bool>> predicateLast, int pageSize, byte[] pagingStateFirst, byte[] pagingStateLast)
        {
            var resultFirst = await Entities
                .Where(predicateFirst)
                .OrderByDescending(x => x.CreatedAt)
                .SetPageSize(pageSize)
                .SetPagingState(pagingStateFirst)
                .ExecutePagedAsync();

            var resultLast = await Entities
                .Where(predicateLast)
                .OrderByDescending(x => x.CreatedAt)
                .SetPageSize(pageSize)
                .SetPagingState(pagingStateLast)
                .ExecutePagedAsync();

            var combinedResult = resultFirst.Concat(resultLast)
                .OrderByDescending(x => x.CreatedAt)
                .Take(pageSize);


            string pagingStateFirstBase64 = (resultFirst.Count() != 0 && resultFirst.PagingState != null) ? Convert.ToBase64String(resultFirst.PagingState) : string.Empty;
            string pagingStateLastBase64 = (resultLast.Count() != 0 && resultLast.PagingState != null) ? Convert.ToBase64String(resultLast.PagingState) : string.Empty;

            return (combinedResult, (pagingStateFirstBase64, pagingStateLastBase64));
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