using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using Dapper;
using System.Data;
using System.Text;

namespace CabPostService.Infrastructures.Repositories
{
    public class SharePostRepository : PostgresBaseRepository, ISharePostRepository
    {
        private const string ERROR_MESSAGE = "The PostShare Id type is not valid";
        private readonly ILogger<SharePostRepository> _logger;

        private const string SELECT_QUERY = "SELECT * FROM \"SharePosts\" WHERE \"Id\" = @Id";
        private const string SELECT_BY_USER_ID_QUERY = "SELECT * FROM \"SharePosts\" WHERE \"UserId\" = @UserId";
        private const string DELETE_QUERY = "DELETE FROM \"SharePosts\" WHERE \"Id\" = @Id";
        private const string INSERT_QUERY = "INSERT INTO \"SharePosts\" (\"Id\", \"UserId\", \"SharedUserId\", \"ShareLink\", \"IsPublic\", \"PostId\",\"UpdatedAt\", \"CreatedAt\")  VALUES(@Id, @UserId, @SharedUserId, @ShareLink, @IsPublic, @PostId, @UpdatedAt, @CreatedAt)";
        private const string COUNT_QUERY = "SELECT COUNT(*) FROM \"SharePosts\" WHERE \"UserId\" = @UserId";
        #region ctos

        public SharePostRepository(
          IConfiguration configuration,
          ILogger<SharePostRepository> logger)
          : base(configuration)
        {
            _logger = logger;
        }

        #endregion
        public async Task<SharePost> GetByIdAsync<IdType>(IdType id)
        {
            if (id is not Guid)
                throw new Exception(ERROR_MESSAGE);

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);

            using var connection = CreateConnection();

            return await connection.QueryFirstOrDefaultAsync<SharePost>(SELECT_QUERY, parameters);
        }

        public async Task<int> CreateAsync(SharePost entity)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", entity.Id, DbType.Guid);
            parameters.Add("UserId", entity.UserId, DbType.Guid);
            parameters.Add("SharedUserId", entity.SharedUserId, DbType.Guid);
            parameters.Add("ShareLink", entity.ShareLink, DbType.String);
            parameters.Add("PostId", entity.PostId, DbType.String);
            parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);
            parameters.Add("CreatedAt", entity.CreatedAt, DbType.DateTime2);
            parameters.Add("IsPublic", entity.IsPublic, DbType.Boolean);
            
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(INSERT_QUERY, parameters);
        }

        public async Task<int> UpdateAsync(SharePost entity)
        {
            return await Task.FromResult(0);
        }

        public async Task<int> DeleteAsync<IdType>(IdType id)
        {
            if (id is not Guid)
                throw new Exception(ERROR_MESSAGE);

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);

            using var connection = CreateConnection();
            return await connection.ExecuteAsync(DELETE_QUERY, parameters);
        }

        public async Task<IEnumerable<SharePost>> GetAllPostShareByFilterAsync(SharePostFilter sharePostFilter)
        {
            var query = new StringBuilder(SELECT_BY_USER_ID_QUERY);
            query.Append(sharePostFilter.PagingQueryString);

            var parameters = new DynamicParameters();
            parameters.Add("UserId", sharePostFilter, DbType.Guid);

            using var connection = CreateConnection();

            return await connection.QueryAsync<SharePost>(query.ToString(), parameters);
        }

        public async Task<long> GetTotalAsync(SharePostFilter sharePostFilter)
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", sharePostFilter, DbType.Guid);
            using var connection = CreateConnection();

            return await connection.QueryFirstAsync<long>(COUNT_QUERY, parameters);
        }
    }
}
