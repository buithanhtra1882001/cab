using CabPaymentService.Infrastructures.DbContexts;
using CabPaymentService.Infrastructures.Repositories.Base;
using CabPaymentService.Infrastructures.Repositories.Interfaces;
using CabPaymentService.Model.Entities;

namespace CabPaymentService.Infrastructures.Repositories;
public class VnPayTransactionRepository : PostgresBaseRepository<VnPayTransaction, Guid>, IVnPayTransactionRepository
{
    public VnPayTransactionRepository(PostgresDbContext context) : base(context)
    {
    }
}