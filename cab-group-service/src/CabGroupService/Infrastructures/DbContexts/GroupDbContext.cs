using CabGroupService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CabGroupService.Infrastructures.DbContexts
{
    public class GroupDbContext: DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Group> Group { get; set; }
        public DbSet<GroupMembers> GroupMembers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseNpgsql(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("GroupDbConnectionString").Value);
           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GroupMembers>().HasKey(gm => new { gm.GroupID, gm.UserID });
        }
    }
}
