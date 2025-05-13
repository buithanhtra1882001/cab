using CabMediaService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CabMediaService.Infrastructures.DbContexts
{
    public class PostgresDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<MediaImage> MediaImage { get; set; }
        public PostgresDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseNpgsql(_configuration.GetValue<string>("MediaDbConnectionString"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
