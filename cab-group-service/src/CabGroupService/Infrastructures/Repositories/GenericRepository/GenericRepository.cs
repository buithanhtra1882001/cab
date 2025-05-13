using CabGroupService.Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CabGroupService.Infrastructures.Repositories.GenericRepository
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected GroupDbContext _context;
        protected DbSet<TEntity> _dbSet;
        public GenericRepository(IUnitOfWork unitOfWork)
        {
            _context = unitOfWork.Context;
            _dbSet = _context.Set<TEntity>();
        }
        public virtual async Task Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual async Task Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
                _dbSet.Attach(entityToDelete);
            _dbSet.Remove(entityToDelete);
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            if (orderBy != null)
                return orderBy(query).ToList();
            return query.ToList();
        }

        public virtual async Task<TEntity> GetByID(object id) => _dbSet.Find(id);

        public virtual async Task Insert(TEntity entity) => _dbSet.Add(entity);

        public virtual async Task Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}
