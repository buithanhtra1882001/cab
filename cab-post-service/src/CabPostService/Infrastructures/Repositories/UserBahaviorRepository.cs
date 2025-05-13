using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;
using Dapper;
using System.Data;

namespace CabPostService.Infrastructures.Repositories
{
    public class UserBahaviorRepository : PostgresBaseRepository,
        IUserBahaviorRepository
    {
        private readonly ILogger<UserBahaviorRepository> _logger;

        public UserBahaviorRepository(
            IConfiguration configuration,
            ILogger<UserBahaviorRepository> logger)
            : base(configuration)
        {
            _logger = logger;
        }

        public async Task<int> AddUserBahavior(UserBehavior entity)
        {
            var query = "INSERT INTO \"UserBehaviors\" (\"Id\", \"UserId\", \"PostId\", \"Type\", \"IsHidden\", \"UpdatedAt\", \"CreatedAt\")" +
                " VALUES(@Id, @UserId, @PostId, @Type, @IsHidden, @UpdatedAt, @CreatedAt)";
            var parameters = new DynamicParameters();
            parameters.Add("Id", entity.Id, DbType.Guid);
            parameters.Add("UserId", entity.UserId, DbType.Guid);
            parameters.Add("PostId", entity.PostId, DbType.String);
            parameters.Add("Type", entity.Type, DbType.Int32);
            parameters.Add("IsHidden", entity.IsHidden, DbType.Boolean);
            parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);
            parameters.Add("CreatedAt", entity.CreatedAt, DbType.DateTime2);

            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query, parameters);

        }
    }
}
