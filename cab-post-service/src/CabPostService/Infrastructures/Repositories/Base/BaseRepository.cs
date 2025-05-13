using AutoMapper.Internal;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities.Base;
using Cassandra.Data.Linq;
using System.Linq.Expressions;

namespace CabPostService.Infrastructures.Repositories.Base
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
            var result = await Entities
                .FirstOrDefault(predicate)
                .ExecuteAsync();

            ConvertDateTimeUTC(result);

            return result;
        }

        public virtual async Task<T> GetLastOneAsync()
        {
            var result = await Entities
                .OrderByDescending(e => e.CreatedAt)
                .FirstOrDefault()
                .ExecuteAsync();

            ConvertDateTimeUTC(result);

            return result;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var result = await Entities.ExecuteAsync();

            ConvertDateTimeUTC(result);

            return result;
        }

        public virtual async Task<IEnumerable<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            var result = await Entities
                .Where(predicate)
                .ExecuteAsync();

            //ConvertDateTimeUTC(result);

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

            string pagingStateBase64String = (result.Any() && result.Count() == pageSize) ? Convert.ToBase64String(result.PagingState) : string.Empty;

            ConvertDateTimeUTC(result);

            return (result, pagingStateBase64String);
        }

        public virtual async Task<long> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await Entities
                .Where(predicate)
                .Count()
                .ExecuteAsync();
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            ValidateAndThrow(entity);
            await Entities
                .Insert(entity)
                .ExecuteAsync();

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
            await Entities
                .Where(predicate)
                .Delete()
                .ExecuteAsync();

            return true;
        }

        public virtual async Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            await Entities
                .Where(predicate)
                .Delete()
                .ExecuteAsync();

            return true;
        }

        #endregion public

        #region private

        private static void ValidateAndThrow(T entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));
        }

        private static void AddDefaultValue(ref T entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = entity.CreatedAt;
        }

        private static void ConvertDateTimeUTC(T entity)
        {
            try
            {
                entity.CreatedAt = entity.CreatedAt.ToLocalTime();
                entity.UpdatedAt = entity.CreatedAt.ToLocalTime();
            }
            catch
            {
                return;
            }
        }

        private static void ConvertDateTimeUTC(IEnumerable<T> entities)
        {
            entities.ForAll(e => ConvertDateTimeUTC(e));
        }

        protected Table<T> Entities
        {
            get
            {
                _entitiesDbSet ??= _context.GetTable<T>();

                return _entitiesDbSet;
            }
        }
        #endregion private
    }
}