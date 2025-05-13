using CabPaymentService.Model.Entities;

namespace CabPaymentService.Infrastructures.Repositories.Interfaces
{
    public interface IVnPayTransactionRepository : IPostgresBaseRepository<VnPayTransaction, Guid>
    {
    }
}
