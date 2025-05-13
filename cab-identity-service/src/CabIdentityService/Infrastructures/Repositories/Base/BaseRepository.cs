using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.Infrastructures.Repositories.Interfaces;
using WCABNetwork.Cab.IdentityService.Models.Entities.Base;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.Repositories.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        #region props

        private readonly DbContext _context;
        private bool _disposed = false;
        private DbSet<T> _entitiesDbSet { get; set; }
        private IDbContextTransaction _tx { get; set; }

        #endregion props

        #region ctor

        public BaseRepository(DbContext context)
        {
            _context = context;
        }

        ~BaseRepository()
        {
            Dispose(false);
        }

        #endregion ctor

        #region public

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = await GetNoTrackingEntities().ToListAsync();
            return entities;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            var entity = await GetNoTrackingEntities().FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }

        public virtual async Task<T> GetByUuidAsync(string uuid)
        {
            var entity = await GetNoTrackingEntities().FirstOrDefaultAsync(x => x.Uuid == uuid);
            return entity;
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            ValidateAndThrow(entity);
            AddDefaultValue(ref entity);
            Entities.Add(entity);

            var effectedCount = await _context.SaveChangesAsync();
            if (effectedCount == 0)
            {
                return null;
            }
            return entity;
        }

        public virtual async Task<List<T>> CreateAsync(List<T> entities)
        {
            for (int i = 0; i < entities.Count; i++)
            {
                var entity = entities[i];
                ValidateAndThrow(entity);
                AddDefaultValue(ref entity);
            }

            Entities.AddRange(entities);
            var effectedCount = await _context.SaveChangesAsync();
            if (effectedCount == 0)
            {
                return null;
            }
            return entities;
        }

        public virtual async Task<bool> UpdateAsync(T entity)
        {
            ValidateAndThrow(entity);
            var entry = _context.Entry(entity);
            if (entry.State < EntityState.Added)
            {
                entry.State = EntityState.Modified;
            }

            entity.UpdatedAt = DateTime.UtcNow;
            var effectedCount = await _context.SaveChangesAsync();
            return effectedCount > 0;
        }

        public virtual async Task<bool> DeleteAsync(T entity)
        {
            ValidateAndThrow(entity);
            Entities.Remove(entity);
            var effectedCount = await _context.SaveChangesAsync();
            return effectedCount > 0;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion public

        #region private

        private void ValidateAndThrow(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
        }

        private void AddDefaultValue(ref T entity)
        {
            entity.Uuid = Guid.NewGuid().ToString();
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = entity.CreatedAt;
        }

        protected DbSet<T> Entities
        {
            get
            {
                if (_entitiesDbSet == null)
                    _entitiesDbSet = _context.Set<T>();
                return _entitiesDbSet;
            }
        }

        protected IQueryable<T> GetNoTrackingEntities()
        {
            var table = Entities.AsNoTracking();
            return table;
        }

        protected async Task BeginTransactionAsync()
        {
            _tx = await _context.Database.BeginTransactionAsync();
        }

        protected async Task CommitTransactionAsync()
        {
            await _tx.CommitAsync();
            await ReleaseTransactionAsync();
        }

        protected async Task RollbackTransactionAsync()
        {
            await _tx.RollbackAsync();
            await ReleaseTransactionAsync();
        }

        protected async Task ReleaseTransactionAsync()
        {
            await _tx.DisposeAsync();
            _tx = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _tx?.Dispose();
                _tx = null;
            }

            _disposed = true;
        }

        #endregion private
    }
}