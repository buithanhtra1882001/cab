using CabUserService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CabUserService.Infrastructures.DbContexts
{
    public class PostgresDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<User> Users { get; set; }
        public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<UserCategory> UserCategories { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<UserBalanceLog> UserBalanceLogs { get; set; }
        public DbSet<UserTransaction> UserTransactionLogs { get; set; }
        public DbSet<UserRequestFriendAction> UserRequestFriendActions { get; set; }
        public DbSet<UserFriend> UserFriends { get; set; }
        public DbSet<UserViewProfileHistory> UserViewProfileHistories { get; set; }
        public DbSet<UserFollowHistory> UserFollowHistories { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ChatUserConnection> ChatUserConnections { get; set; }
        public DbSet<DonateReceiverRequest> DonateReceiverRequests { get; set; }
        public DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }
        public PostgresDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseNpgsql(_configuration.GetValue<string>("UserDbConnectionString"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDetail>().HasKey(e => e.UserDetailId);

            modelBuilder.Entity<UserFriend>().HasKey(e => new
            {
                e.UserId,
                e.FriendId
            });

            modelBuilder.Entity<UserDetail>()
                .HasOne(userDetail => userDetail.User)
                .WithOne(user => user.UserDetail)
                .HasForeignKey<UserDetail>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
