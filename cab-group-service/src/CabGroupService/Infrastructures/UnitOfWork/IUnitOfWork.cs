using CabGroupService.Infrastructures.DbContexts;

namespace CabGroupService.Infrastructures
{
    public interface IUnitOfWork
    {
        GroupDbContext Context { get; }
        void CreateTransaction();
        void Commit();
        void Rollback();
        void Save();
    }
}
