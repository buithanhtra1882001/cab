using CabPaymentService.Infrastructures.DbContexts;
using CabPaymentService.Infrastructures.Repositories.Base;
using CabPaymentService.Model.Entities;

namespace CabPaymentService.Infrastructures.Repositories;

public class InvoiceRepository : PostgresBaseRepository<Invoice, Guid>
{
    public InvoiceRepository(PostgresDbContext context) : base(context)
    {
    }
}

