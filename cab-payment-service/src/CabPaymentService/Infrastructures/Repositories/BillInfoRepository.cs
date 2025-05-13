using CabPaymentService.Infrastructures.DbContexts;
using CabPaymentService.Infrastructures.Repositories.Base;
using CabPaymentService.Model.Entities;

namespace CabPaymentService.Infrastructures.Repositories;

public class BillInfoRepository : PostgresBaseRepository<BillInfo, Guid>
{
    public BillInfoRepository(PostgresDbContext context) : base(context)
    {
        
    }
}