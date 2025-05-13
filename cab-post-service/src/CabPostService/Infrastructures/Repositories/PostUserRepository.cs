using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostUserRepository :
       PostgresBaseRepository, IPostUserRepository
    {
        private readonly ILogger<PostUserRepository> _logger;
        public PostUserRepository(
           IConfiguration configuration,
           ILogger<PostUserRepository> logger)
           : base(configuration)
        {
            _logger = logger;
        }

        public async Task<int> CreateAsync(PostUsers entity)
        {
            using var connection = CreateConnection();
            var query = "INSERT INTO \"PostUsers\" (\"PostId\", \"UserId\", \"IsShowPost\") VALUES (@PostId, @UserId, @IsShowPost)";

            return await connection.ExecuteAsync(query, new { PostId = entity.PostId, UserId = entity.UserId, IsShowPost = entity.IsShowPost });
        }
        
        public async Task<int> UpdateAsync(PostUsers entity)
        {
            using var connection = CreateConnection();
            var query = "UPDATE \"PostUsers\" SET \"PostId\" = @PostId, \"UserId\" = @UserId, \"IsShowPost\" = @IsShowPost WHERE \"PostId\" = @PostId AND \"UserId\" = @UserId";

            return await connection.ExecuteAsync(query, new { PostId = entity.PostId, UserId = entity.UserId, IsShowPost = entity.IsShowPost });
        }

        public async Task<PostUsers> GetByUserIdAndPostId(Guid userId, string postId)
        {
            using var connection = CreateConnection();
            var query = new StringBuilder("SELECT * FROM \"PostUsers\" WHERE \"PostId\" = @PostId AND \"UserId\" = @UserId");
            
            var parameters = new DynamicParameters();
            parameters.Add("PostId", postId, DbType.String);
            parameters.Add("UserId", userId, DbType.Guid);
            
            return await connection.QueryFirstOrDefaultAsync<PostUsers>(query.ToString(), parameters);
        }


        public async Task<bool> CheckExist(string postId, Guid userId)
        {
            using var connection = CreateConnection();
            var query = new StringBuilder("SELECT COUNT(0) FROM \"PostUsers\" WHERE \"PostId\" = @PostId AND \"UserId\" = @UserId");
            
            var parameters = new DynamicParameters();
            parameters.Add("PostId", postId, DbType.String);
            parameters.Add("UserId", userId, DbType.Guid);
            
            return await connection.ExecuteScalarAsync<bool>(query.ToString(), parameters);

        }
    }
}
