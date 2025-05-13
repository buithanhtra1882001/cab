using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WCABNetwork.Cab.IdentityService.Models.Entities;

namespace WCABNetwork.Cab.IdentityService.Infrastructures.DbContexts
{
    public class IdentityCoreDbContext : IdentityDbContext<Account, IdentityRole<int>, int>
    {
        private readonly IConfiguration _configuration;
        public DbSet<UserRefreshToken> UserRefreshToken { get; set; }
        public IdentityCoreDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseNpgsql(_configuration.GetValue<string>("IdentityDbConnectionString"));
        }
    }
}
