using CabPaymentService.Infrastructures.Constants;
using CabPaymentService.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace CabPaymentService.Infrastructures.DbContexts
{
    public class PostgresDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<VnPayTransaction> VnpayTransactions { get; set; }
        public DbSet<BillInfo> BillInfos { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        /// <summary>
        /// IConfiguration must specify connection string to connect to separate database
        /// add DbContextOptions to use EF Core in-memory database for unit testing
        /// </summary>
        /// <param name="configuration">contains connection string to an external database</param>
        /// <param name="options">is for EF Core in-memory database for unit testing</param>
        public PostgresDbContext(IConfiguration configuration, DbContextOptions options = null)
            : base(options ?? new DbContextOptions<DbContext>())
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BillInfo>()
                .HasOne(x => x.Transaction)
                .WithOne(x => x.BillInfo)
                .HasForeignKey<VnPayTransaction>(x => x.BillId);
            modelBuilder.Entity<Invoice>()
                .HasOne(x => x.Transaction)
                .WithOne(x => x.Invoice)
                .HasForeignKey<VnPayTransaction>(x => x.InvoiceId);

            modelBuilder.Entity<VnPayTransaction>()
                .Property(e => e.TransactionType)
                .HasConversion(
                    v => v.ToString(),
                    v => (VnPayTransactionType)Enum.Parse(typeof(VnPayTransactionType), v));

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // IsConfigured = true when using in-memory database for unit test
            // IsConfigured = false when using an external database
            if (!optionsBuilder.IsConfigured) 
            {
                _ = optionsBuilder.UseNpgsql(_configuration.GetValue<string>("PaymentDbConnectionString"));
            }
        }
    }
}