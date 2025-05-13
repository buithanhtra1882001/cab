using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;
using Dapper;
using System.Data;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostVideoRepository : PostgresBaseRepository, IPostVideoRepository
    {
        private readonly ILogger<PostVideoRepository> _logger;

        public PostVideoRepository(
            IConfiguration configuration,
            ILogger<PostVideoRepository> logger
            ) : base(configuration)
        {
            _logger = logger;
        }

        public async Task<int> CreateAsync(PostVideo entity)
        {
            var query = "INSERT INTO \"PostVideos\" (\"Id\", \"UserId\", \"MediaVideoId\", \"VideoUrl\", \"Description\", \"LengthVideo\", \"AvgViewLength\", \"IsViolence\", \"ViewCount\" , \"UpdatedAt\", \"CreatedAt\")  " +
                        "VALUES(@Id, @UserId, @MediaVideoId, @VideoUrl, @Description, @LengthVideo, @AvgViewLength, @IsViolence, @ViewCount, @UpdatedAt, @CreatedAt)";
            var parameters = new DynamicParameters();
            parameters.Add("Id", entity.Id, DbType.String);
            parameters.Add("UserId", entity.UserId, DbType.Guid);
            parameters.Add("MediaVideoId", entity.MediaVideoId, DbType.String);
            parameters.Add("VideoUrl", entity.VideoUrl, DbType.String);
            parameters.Add("Description", entity.Description, DbType.String);
            parameters.Add("LengthVideo", entity.LengthVideo);
            parameters.Add("AvgViewLength", entity.AvgViewLength);
            parameters.Add("IsViolence", entity.IsViolence, DbType.Boolean);
            parameters.Add("ViewCount", entity.ViewCount);
            parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);
            parameters.Add("CreatedAt", entity.CreatedAt, DbType.DateTime2);

            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query, parameters);
        }

        public async Task<string> UpdateAVTP(string postVideoId, double avtp, DateTime updateAt)
        {
            try
            {
                var query = "UPDATE \"PostVideos\" SET \"AvgViewLength\" = @AvgViewLength,\"UpdatedAt\" = @UpdatedAt, \"ViewCount\" = \"ViewCount\" + 1 WHERE \"Id\" = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", postVideoId, DbType.String);
                parameters.Add("AvgViewLength", avtp);
                parameters.Add("UpdatedAt", updateAt);

                using var connection = CreateConnection();
                await connection.ExecuteAsync(query, parameters);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return string.Empty;
        }

        public async Task<PostVideo> GetByIdAsync<IdType>(IdType id)
        {
            if (id == null || id.GetType() != typeof(string))
                throw new Exception("The PostVideoId type is not valid");

            var query = "SELECT * FROM \"PostVideos\" WHERE \"Id\" = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.String);

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PostVideo>(query, parameters);
        }

        public async Task<List<PostVideo>> GetByPostIdsAsync<IdType>(IdType postId)
        {
            if (postId == null || postId.GetType() != typeof(string))
                throw new Exception("The PostVideoId type is not valid");

            var query = "SELECT * FROM \"PostVideos\" WHERE \"PostId\" = (@PostId)";
            var parameters = new DynamicParameters();
            parameters.Add("PostId", postId, DbType.String);

            using var connection = CreateConnection();
            return (await connection.QueryAsync<PostVideo>(query, parameters)).ToList();
        }
    }
}

