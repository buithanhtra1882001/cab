using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using Dapper;
using System.Data;
using System.Text;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostHashtagRepository :
        PostgresBaseRepository,
        IPostHashtagRepository
    {
        private readonly ILogger<PostHashtagRepository> _logger;

        public PostHashtagRepository(
            IConfiguration configuration,
            ILogger<PostHashtagRepository> logger)
            : base(configuration)
        {
            _logger = logger;
        }

        public async Task<List<PostHashtag>> GetByName(string name)
        {
            var query = new StringBuilder("SELECT  \"Id\", \"Slug\", \"Name\", \"Description\", \"IsActived\",  \"Point\",  \"UpdatedAt\", \"CreatedAt\" FROM \"PostHashtags\" WHERE upper(\"Name\") = @Name");

            var parameters = new DynamicParameters();
            parameters.Add("Name", name.ToUpper());

            using var connection = CreateConnection();
            return (await connection.QueryAsync<PostHashtag>(query.ToString(), parameters)).ToList();
        }

        public async Task<List<PostHashtag>> SearchDataBySlug(string slug)
        {
            var query = new StringBuilder("SELECT  \"Id\", \"Slug\", \"Name\", \"Description\", \"IsActived\",  \"Point\",  \"UpdatedAt\", \"CreatedAt\" FROM \"PostHashtags\" WHERE upper(\"Slug\") LIKE  '%' || @Slug || '%'");

            var parameters = new DynamicParameters();
            parameters.Add("Slug", slug.ToUpper());

            using var connection = CreateConnection();
            return (await connection.QueryAsync<PostHashtag>(query.ToString(), parameters)).ToList();
        }

        public async Task<List<PostHashtag>> GetDataByLimit(int limit)
        {
            var query = new StringBuilder("SELECT  \"Id\", \"Slug\", \"Name\", \"Description\", \"IsActived\",  \"Point\",  \"UpdatedAt\", \"CreatedAt\" FROM \"PostHashtags\" ORDER BY  \"Point\" DESC  LIMIT @limit ");

            var parameters = new DynamicParameters();
            parameters.Add("limit", limit);

            using var connection = CreateConnection();
            return (await connection.QueryAsync<PostHashtag>(query.ToString(), parameters)).ToList();
        }

        public async Task<int> UpdateAsync(PostHashtag entity)
        {
            var query = "UPDATE \"PostHashtags\" SET \"Slug\" = @Slug, \"Name\" = @Name, \"Description\" = @Description, \"IsActived\" = @IsActived,  \"Point\" = @Point,  \"UpdatedAt\" = @UpdatedAt  WHERE \"Id\" = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", entity.Id);
            parameters.Add("Slug", entity.Slug);
            parameters.Add("Name", entity.Name);
            parameters.Add("Description", entity.Description);
            parameters.Add("IsActived", entity.IsActived);
            parameters.Add("Point", entity.Point);
            parameters.Add("UpdatedAt", entity.UpdatedAt);
            parameters.Add("CreatedAt", entity.CreatedAt);

            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query, parameters);
        }

        #region Base Repository
        public async Task<PostHashtag> GetByIdAsync<IdType>(IdType id)
        {
            var query = "SELECT * FROM \"PostHashtags\" WHERE \"Id\" = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PostHashtag>(query, parameters);
        }

        public async Task UpdateMultipleAsync(List<PostHashtag> list)
        {
            foreach (var entity in list)
            {
                var query = "UPDATE \"PostHashtags\" SET \"Slug\" = @Slug, \"Name\" = @Name, \"Description\" = @Description, \"IsActived\" = @IsActived,  \"Point\" = @Point,  \"UpdatedAt\" = @UpdatedAt  WHERE \"Id\" = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", entity.Id);
                parameters.Add("Slug", entity.Slug);
                parameters.Add("Name", entity.Name);
                parameters.Add("Description", entity.Description);
                parameters.Add("IsActived", entity.IsActived);
                parameters.Add("Point", entity.Point);
                parameters.Add("UpdatedAt", entity.UpdatedAt);
                parameters.Add("CreatedAt", entity.CreatedAt);

                using var connection = CreateConnection();
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<int> CreateAsync(PostHashtag entity)
        {
            var query = "INSERT INTO \"PostHashtags\" (\"Id\", \"Slug\", \"Name\", \"Description\", \"IsActived\", \"Point\", \"UpdatedAt\", \"CreatedAt\")  VALUES(@Id, @Slug, @Name, @Description, @IsActived, @Point, @UpdatedAt, @CreatedAt)";
            var parameters = new DynamicParameters();
            parameters.Add("Id", entity.Id);
            parameters.Add("Slug", entity.Slug);
            parameters.Add("Name", entity.Name);
            parameters.Add("Description", entity.Description);
            parameters.Add("IsActived", entity.IsActived);
            parameters.Add("Point", entity.Point);
            parameters.Add("UpdatedAt", entity.UpdatedAt);
            parameters.Add("CreatedAt", entity.CreatedAt);

            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query, parameters);
        }

        public async Task<int> DeleteAsync<IdType>(IdType id)
        {
            if (id == null || id.GetType() != typeof(string))
                throw new Exception("The Post Id type is not valid");

            var query = "DELETE FROM \"Posts\" WHERE \"Id\" = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.String);

            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query, parameters);
        }
        #endregion
    }
}