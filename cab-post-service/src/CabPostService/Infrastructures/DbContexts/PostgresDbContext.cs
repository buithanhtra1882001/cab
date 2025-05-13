using CabPostService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.Infrastructures.DbContexts
{
    public class PostgresDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostVideo> PostVideos { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<PostHashtag> PostHashtags { get; set; }
        public DbSet<PostCategoryType> PostCategoryType { get; set; }
        public DbSet<PostVote> PostVotes { get; set; }
        public DbSet<SharePost> SharePosts { get; set; }
        public DbSet<PostMiniUser> Users { get; set; }
        public DbSet<PostUsers> PostUsers { get; set; }
        public DbSet<PostNotifyAdmin> PostNotifyAdmin { get; set; }
        public DbSet<UserBehavior> UserBehaviors { get; set; }
        public PostgresDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseNpgsql(_configuration.GetValue<string>("PostDbConnectionString"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PostVote>()
                .HasKey(pv => new { pv.Id, pv.PostId, pv.UserVoteId });

            builder.Entity<PostUsers>()
                .HasKey(pu => new { pu.PostId, pu.UserId });
        }
    }
}
