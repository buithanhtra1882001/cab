using CabGroupService.Infrastructures.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace CabGroupService.Infrastructures
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private GroupDbContext _context;
        private IDbContextTransaction _transaction;
        private bool disposed = false;
        public UnitOfWork()
        {
            _context = new GroupDbContext();
        }
        public GroupDbContext Context
        {
            get { return _context; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                    _context.Dispose();
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void CreateTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void Rollback()
        {
            _transaction.Rollback();
            _transaction.Dispose();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
