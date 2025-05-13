using CabPostService.Constants;
using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using CabPostService.Models.Queries;
using Dapper;
using MediatR;
using System.Data;
using System.Text;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostRepository :
        PostgresBaseRepository,
        IPostRepository
    {
        private readonly ILogger<PostRepository> _logger;

        public PostRepository(
            IConfiguration configuration,
            ILogger<PostRepository> logger)
            : base(configuration)
        {
            _logger = logger;
        }

        public void InsertPosts(List<Post> posts)
        {
            using var connection = CreateConnection();

            posts.ForEach((post) =>
            {
                connection.Execute(@"
                    INSERT INTO ""Posts"" (""Id"", ""UserId"", ""PostType"", ""CategoryId"", ""Hashtags"", ""Content"", ""ImageIds"", ""VideoIds""
, ""Point"", ""LikesCount"", ""CommentsCount"", ""Status"", ""IsDonateOpen"", ""IsChecked"", ""IsSoftDeleted"", ""Title"", ""UpdatedAt"", ""CreatedAt"",""AdminBoost"",""ViewCount"",""VoteUpCount"",""VoteDownCount"",""PosterScore"") 
                    VALUES (
                    @Id,
                    @UserId,
                    @PostType,
                    @CategoryId,
                    @Hashtags,
                    @Content,
                    @ImageIds,
                    @VideoIds,
                    @Point,
                    @LikesCount,
                    @CommentsCount,
                    @Status,
                    @IsDonateOpen,
                    @IsChecked,
                    @IsSoftDeleted,
                    @Title,
                    @UpdatedAt,
                    @CreatedAt,
                    @AdminBoost,
                    @ViewCount,
                    @VoteUpCount,
                    @VoteDownCount,
                    @PosterScore                   
                    )", param: post);
            });
        }

        public async Task<int> CountPosts()
        {
            using var con = CreateConnection();
            return await con.ExecuteScalarAsync<int>(@"SELECT COUNT(*) FROM ""Posts""");
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

        public async Task<List<Post>> GetAllAsync(GetAllPostFilter filter)
        {
            var prepare = PrepareGetAllQuery(filter);
            var query = new StringBuilder("SELECT * FROM \"Posts\"");

            query.Append(prepare.Item1);
            query.Append(filter.PagingQueryString);

            using var connection = CreateConnection();
            return (await connection.QueryAsync<Post>(query.ToString(), prepare.Item2)).ToList();
        }
        public async Task<List<GetAllPostResponse>> GetOrderedPostsAsync(GetAllPostOrderByPointWithPaging pagding)
        {
            var query = new StringBuilder("");
            var parameters = new DynamicParameters();
            parameters.Add("Status", 1);
            parameters.Add("IsSoftDeleted", false);
            parameters.Add("IsChecked", true);
            //List of post in a group
            if (pagding.GroupId != null)
            {
                if(!Guid.TryParse(pagding.GroupId, out _))
                    throw new Exception("The provided GroupId is not a valid GUID.");

                query = new StringBuilder("SELECT DISTINCT P.*, U.\"Fullname\" AS UserFullName, U.\"Avatar\" AS UserAvatar, PV.\"Type\" AS VoteType " +
                                          "FROM \"Posts\" P " +
                                          "LEFT JOIN \"Users\" U ON P.\"UserId\" = U.\"Id\" " +
                                          "LEFT JOIN \"PostVotes\" PV ON P.\"Id\" = PV.\"PostId\" AND U.\"Id\" = PV.\"UserVoteId\" " +
                                          "WHERE \"Status\" = @Status AND \"IsSoftDeleted\" = @IsSoftDeleted AND \"IsChecked\" = @IsChecked AND \"IsPersonalPost\" = @IsPersonalPost AND \"GroupId\" = @GroupId AND \"IsApproved\" = @IsApproved ");
                parameters.Add("IsPersonalPost", false);
                parameters.Add("GroupId", pagding.GroupId);
                parameters.Add("IsApproved", pagding.GroupPostStatus);
            }
            else
            {
                query = new StringBuilder("SELECT DISTINCT P.*, U.\"Fullname\" AS UserFullName, U.\"Avatar\" AS UserAvatar, PV.\"Type\" AS VoteType " +
                                          "FROM \"Posts\" P " +
                                          "LEFT JOIN \"Users\" U ON P.\"UserId\" = U.\"Id\" " +
                                          "LEFT JOIN \"PostVotes\" PV ON P.\"Id\" = PV.\"PostId\" AND U.\"Id\" = PV.\"UserVoteId\" " +
                                          "WHERE \"Status\" = @Status AND \"IsSoftDeleted\" = @IsSoftDeleted AND \"IsChecked\" = @IsChecked AND \"IsPersonalPost\" = @IsPersonalPost AND \"IsApproved\" = @IsApproved ");
                parameters.Add("IsPersonalPost", true);
                parameters.Add("IsApproved", 1);
            }
            query.Append(pagding.PagingQueryString);

            using var connection = CreateConnection();
            return (await connection.QueryAsync<GetAllPostResponse>(query.ToString(), parameters)).ToList();
        }

        public async Task<List<GetAllPostResponse>> GetPostsByUserIdAsync(GetPostByUserIdQuery request)
        {
            var query = new StringBuilder("");
            var parameters = new DynamicParameters();
            parameters.Add("UserId", request.UserId);
            parameters.Add("Status", 1);
            parameters.Add("IsSoftDeleted", false);
            parameters.Add("IsChecked", true);
            parameters.Add("Skip", (request.PageNumber - 1) * request.PageSize);
            parameters.Add("PageSize", request.PageSize);
            if (request.GroupId != null)
            {
                if (!Guid.TryParse(request.GroupId, out _))
                    throw new Exception("The provided GroupId is not a valid GUID.");
                query = new StringBuilder("SELECT DISTINCT P.*, U.\"Fullname\" AS UserFullName, U.\"Avatar\" AS UserAvatar, PV.\"Type\" AS VoteType " +
                                         "FROM \"Posts\" P " +
                                         "LEFT JOIN \"Users\" U ON P.\"UserId\" = U.\"Id\" " +
                                         "LEFT JOIN \"PostVotes\" PV ON P.\"Id\" = PV.\"PostId\" AND U.\"Id\" = PV.\"UserVoteId\" " +
                                         "WHERE P.\"UserId\" = @UserId AND \"Status\" = @Status AND \"IsSoftDeleted\" = @IsSoftDeleted AND \"IsChecked\" = @IsChecked AND \"IsPersonalPost\" = @IsPersonalPost AND \"IsApproved\" = @IsApproved AND \"GroupId\" = @GroupId ORDER BY P.\"CreatedAt\" DESC OFFSET @Skip LIMIT @PageSize ");
                parameters.Add("GroupId", request.GroupId);
                parameters.Add("IsPersonalPost", false);
                parameters.Add("IsApproved", GroupPostStatus.APPROVED);
      

            }
            else
            {
                query = new StringBuilder("SELECT DISTINCT P.*, U.\"Fullname\" AS UserFullName, U.\"Avatar\" AS UserAvatar, PV.\"Type\" AS VoteType " +
                                         "FROM \"Posts\" P " +
                                         "LEFT JOIN \"Users\" U ON P.\"UserId\" = U.\"Id\" " +
                                         "LEFT JOIN \"PostVotes\" PV ON P.\"Id\" = PV.\"PostId\" AND U.\"Id\" = PV.\"UserVoteId\" " +
                                         "WHERE P.\"UserId\" = @UserId AND \"Status\" = @Status AND \"IsSoftDeleted\" = @IsSoftDeleted AND \"IsChecked\" = @IsChecked AND \"IsPersonalPost\" = @IsPersonalPost AND \"IsApproved\" = @IsApproved ORDER BY P.\"CreatedAt\" DESC OFFSET @Skip LIMIT @PageSize ");
                parameters.Add("IsPersonalPost", true);
                parameters.Add("IsApproved", 1);
            }
                using var connection = CreateConnection();
            return (await connection.QueryAsync<GetAllPostResponse>(query.ToString(), parameters)).ToList();
        }


        public async Task<Post> GetByIdAsync<IdType>(IdType id)
        {
            if (id == null || id.GetType() != typeof(string))
                throw new Exception("The Post Id type is not valid");

            var query = "SELECT * FROM \"Posts\" WHERE \"Id\" = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.String);

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Post>(query, parameters);
        }

        public async Task<int> UpdateAsync(Post entity)
        {
            var query = "UPDATE \"Posts\" SET \"UserId\" = @UserId, \"Title\" = @Title, \"PostType\" = @PostType, \"CategoryId\" = @CategoryId, \"Hashtags\" = @Hashtags, \"Content\" = @Content, \"ImageIds\" = @ImageIds, \"VideoIds\" = @VideoIds, \"Point\" = @Point, \"Status\" = @Status, \"IsDonateOpen\" = @IsDonateOpen, \"IsChecked\" = @IsChecked, \"IsSoftDeleted\" = @IsSoftDeleted, \"UpdatedAt\" = @UpdatedAt, \"VoteUpCount\" = @VoteUpCount, \"VoteDownCount\" = @VoteDownCount WHERE \"Id\" = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", entity.Id, DbType.String);
            parameters.Add("UserId", entity.UserId, DbType.Guid);
            parameters.Add("Title", entity.Title, DbType.String);
            parameters.Add("PostType", entity.PostType, DbType.String);
            parameters.Add("CategoryId", entity.CategoryId, DbType.Guid);
            parameters.Add("Hashtags", entity.Hashtags);
            parameters.Add("Content", entity.Content, DbType.String);
            parameters.Add("ImageIds", entity.ImageIds);
            parameters.Add("VideoIds", entity.VideoIds);
            parameters.Add("Point", entity.Point, DbType.Int32);
            parameters.Add("Status", entity.Status, DbType.Int32);
            parameters.Add("IsDonateOpen", entity.IsDonateOpen, DbType.Boolean);
            parameters.Add("IsChecked", entity.IsChecked, DbType.Boolean);
            parameters.Add("IsSoftDeleted", entity.IsSoftDeleted, DbType.Boolean);
            parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);
            parameters.Add("VoteUpCount", entity.VoteUpCount, DbType.Int32);
            parameters.Add("VoteDownCount", entity.VoteDownCount, DbType.Int32);

            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query, parameters);
        }

        public async Task<int> CreateAsync(Post entity)
        {
            var query = "INSERT INTO \"Posts\" (\"Id\", \"UserId\", \"PostType\", \"CategoryId\", \"Hashtags\", \"Content\", \"ImageIds\", \"VideoIds\", \"Point\", \"LikesCount\", \"CommentsCount\", \"Status\", \"IsDonateOpen\", \"IsChecked\", \"IsSoftDeleted\", \"UpdatedAt\", \"CreatedAt\", \"Title\", \"AdminBoost\", \"ViewCount\", \"VoteUpCount\", \"VoteDownCount\", \"PosterScore\", \"GroupId\", \"IsPersonalPost\", \"IsApproved\")  VALUES(@Id, @UserId, @PostType, @CategoryId, @Hashtags, @Content, @ImageIds, @VideoIds, @Point, @LikesCount, @CommentsCount, @Status, @IsDonateOpen, @IsChecked, @IsSoftDeleted, @UpdatedAt, @CreatedAt, @Title, @AdminBoost, @ViewCount, @VoteUpCount, @VoteDownCount, @PosterScore, @GroupId, @IsPersonalPost, @IsApproved)";
            var parameters = new DynamicParameters();
            parameters.Add("Id", entity.Id, DbType.String);
            parameters.Add("UserId", entity.UserId, DbType.Guid);
            parameters.Add("PostType", entity.PostType, DbType.String);
            parameters.Add("CategoryId", entity.CategoryId, DbType.Guid);
            parameters.Add("Hashtags", entity.Hashtags);
            parameters.Add("Content", entity.Content, DbType.String);
            parameters.Add("ImageIds", entity.ImageIds);
            parameters.Add("VideoIds", entity.VideoIds);
            parameters.Add("Point", entity.Point, DbType.Int32);
            parameters.Add("LikesCount", entity.LikesCount, DbType.Int32);
            parameters.Add("CommentsCount", entity.CommentsCount, DbType.Int32);
            parameters.Add("Status", entity.Status, DbType.Int32);
            parameters.Add("IsDonateOpen", entity.IsDonateOpen, DbType.Boolean);
            parameters.Add("IsChecked", entity.IsChecked, DbType.Boolean);
            parameters.Add("IsSoftDeleted", entity.IsSoftDeleted, DbType.Boolean);
            parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);
            parameters.Add("CreatedAt", entity.CreatedAt, DbType.DateTime2);
            parameters.Add("Title", entity.Title, DbType.String);
            parameters.Add("AdminBoost", entity.AdminBoost, DbType.Boolean);
            parameters.Add("ViewCount", entity.ViewCount, DbType.Int32);
            parameters.Add("VoteUpCount", entity.VoteUpCount, DbType.Int32);
            parameters.Add("VoteDownCount", entity.VoteDownCount, DbType.Int32);
            parameters.Add("PosterScore", entity.PosterScore, DbType.Decimal);
            parameters.Add("GroupId", entity.GroupId, DbType.String);
            parameters.Add("IsPersonalPost", entity.IsPersonalPost, DbType.Boolean);
            parameters.Add("IsApproved", entity.IsApproved, DbType.Int32);
            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query, parameters);
        }

        public async Task<int> UpdateCommentCount(string Id)
        {
            var query = "UPDATE \"Posts\" SET \"CommentsCount\" = \"CommentsCount\" + 1 WHERE \"Id\" = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", Id, DbType.String);

            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query, parameters);
        }

        public async Task<long> GetTotalAsync(GetAllPostFilter filter)
        {
            var prepare = PrepareGetAllQuery(filter);
            var query = new StringBuilder("SELECT COUNT(*) FROM \"Posts\"");
            query.Append(prepare.Item1);

            using var connection = CreateConnection();
            return await connection.QueryFirstAsync<long>(query.ToString(), prepare.Item2);
        }

        private (StringBuilder, DynamicParameters) PrepareGetAllQuery(GetAllPostFilter filter)
        {
            var parameters = new DynamicParameters();
            StringBuilder query = new StringBuilder();
            bool isCombineFilter = false;

            var isUseFilter = !string.IsNullOrEmpty(filter.Slug) ||
                              filter.IsSoftDeleted.HasValue ||
                              filter.IsChecked.HasValue ||
                              filter.Status.HasValue;

            if (isUseFilter)
                query.Append(" WHERE");

            if (!string.IsNullOrEmpty(filter.Slug))
            {
                query.Append(" \"Slug\" = @Slug");
                parameters.Add("Slug", filter.IsSoftDeleted, DbType.String);
                isCombineFilter = true;
            }

            if (filter.IsSoftDeleted.HasValue)
            {
                query.Append(" \"IsSoftDeleted\" = @IsSoftDeleted");
                parameters.Add("IsSoftDeleted", filter.IsSoftDeleted.Value, DbType.Boolean);
                isCombineFilter = true;
            }

            if (filter.IsChecked.HasValue)
            {
                if (isCombineFilter)
                    query.Append(" AND");

                query.Append(" \"IsChecked\" = @IsChecked");
                parameters.Add("IsChecked", filter.IsChecked.Value, DbType.Boolean);
                isCombineFilter = true;
            }

            if (filter.Status.HasValue)
            {
                if (isCombineFilter)
                    query.Append(" AND");

                query.Append(" \"Status\" = @Status");
                parameters.Add("Status", filter.Status.Value, DbType.Int32);
                isCombineFilter = true;
            }

            if (filter.IsPersonalPost.HasValue)
            {
                query.Append(" \"IsPersonalPost\" = @IsPersonalPost");
                parameters.Add("IsPersonalPost", filter.IsPersonalPost.Value, DbType.Boolean);
                isCombineFilter = true;
            }

            return (query, parameters);
        }

        public async Task<List<GetAllPostResponse>> GetPostVideosAsync(GetPostVideosOrderByCreatedWithPaging paging)
        {
            var query = new StringBuilder("");
            var parameters = new DynamicParameters();
            parameters.Add("Status", 1);
            parameters.Add("IsSoftDeleted", false);
            parameters.Add("IsChecked", true);
            query.Append(paging.PagingQueryString);

            if (paging.GroupId != null)
            {
                if (!Guid.TryParse(paging.GroupId, out _))
                    throw new Exception("The provided GroupId is not a valid GUID.");

                parameters.Add("GroupId", paging.GroupId);
                parameters.Add("IsPersonalPost", false);
                parameters.Add("IsApproved", GroupPostStatus.APPROVED);
                query = new StringBuilder("SELECT DISTINCT P.*, U.\"Fullname\" AS UserFullName, U.\"Avatar\" AS UserAvatar " +
                                 "FROM \"Posts\" P " +
                                 "LEFT JOIN \"Users\" U ON P.\"UserId\" = U.\"Id\" " +
                                 "WHERE \"Status\" = @Status AND P.\"PostType\" = 'video' AND \"IsSoftDeleted\" = @IsSoftDeleted AND \"IsChecked\" = @IsChecked AND \"GroupId\" = @GroupId AND \"IsPersonalPost\" = @IsPersonalPost AND \"IsApproved\" = @IsApproved ");
            }
            else
            {
                query = new StringBuilder("SELECT DISTINCT P.*, U.\"Fullname\" AS UserFullName, U.\"Avatar\" AS UserAvatar " +
                                 "FROM \"Posts\" P " +
                                 "LEFT JOIN \"Users\" U ON P.\"UserId\" = U.\"Id\" " +
                                 "WHERE \"Status\" = @Status AND P.\"PostType\" = 'video' AND \"IsSoftDeleted\" = @IsSoftDeleted AND \"IsChecked\" = @IsChecked AND \"IsPersonalPost\" = @IsPersonalPost AND \"IsApproved\" = @IsApproved ");
                parameters.Add("IsPersonalPost", true);
                parameters.Add("IsApproved", 1);
            }

            using var connection = CreateConnection();
            return (await connection.QueryAsync<GetAllPostResponse>(query.ToString(), parameters)).ToList();
        }
    }
}