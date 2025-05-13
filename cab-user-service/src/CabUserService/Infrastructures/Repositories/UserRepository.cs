using CabUserService.Infrastructures.Repositories.Base;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;
using Dapper;
using System.Data;
using System.Text;

namespace CabUserService.Infrastructures.Repositories
{
    public class UserRepository : PostgresBaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task<int> CreateAsync(User entity)
        {
            var query = "INSERT INTO \"Users\" (\"Id\", \"UserName\", \"FullName\", \"Email\", \"SequenceId\", \"Credit\", \"Status\", \"IsSoftDeleted\", \"Coin\", \"UpdatedAt\", \"CreatedAt\") VALUES(@Id, @UserName, @FullName, @Email, @SequenceId, @Credit, @Status, @IsSoftDeleted, @Coin, @UpdatedAt, @CreatedAt)";
            var parameters = new DynamicParameters();
            parameters.Add("Id", entity.Id, DbType.Guid);
            parameters.Add("UserName", entity.UserName, DbType.String);
            parameters.Add("FullName", entity.FullName, DbType.String);
            parameters.Add("Email", entity.Email, DbType.String);
            parameters.Add("SequenceId", entity.SequenceId, DbType.Int64);
            parameters.Add("Credit", entity.Credit, DbType.Int64);
            parameters.Add("Status", entity.Status, DbType.Int16);
            parameters.Add("IsSoftDeleted", entity.IsSoftDeleted, DbType.Boolean);
            parameters.Add("Coin", entity.Coin, DbType.Int64);
            parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);
            parameters.Add("CreatedAt", entity.CreatedAt, DbType.DateTime2);
            using (var connection = CreateConnection())
            {

                try
                {
                    int s = await connection.ExecuteAsync(query, parameters);
                    return s;
                }
                catch (Exception ex)
                {
                    return -1;
                }

            }
        }

        /// <summary>
        /// Only used for unit test cleanup. Use soft delete in production work
        /// </summary>
        /// <typeparam name="IdType"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> HardDeleteAsync<IdType>(IdType id)
        {
            var query = @"DELETE FROM public.""Users"" WHERE ""Id"" = @Id ";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);

            using (var connection = CreateConnection())
            {
                return (await connection.ExecuteAsync(query, parameters));
            }
        }

        public async Task<int> DeleteAsync<IdType>(IdType id)
        {
            var query = "UPDATE \"Users\" SET \"IsSoftDeleted\" = true WHERE \"Id\" = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);

            using (var connection = CreateConnection())
            {
                return (await connection.ExecuteAsync(query, parameters));
            }
        }

        public async Task<List<User>> GetAllAsync(GetAllUserRequest request)
        {
            var prepare = PrepareGetAllQuery(request);
            var query = new StringBuilder("SELECT * FROM \"Users\"");
            query.Append(prepare.Item1);
            query.Append($" ORDER BY \"CreatedAt\" DESC OFFSET {(request.PageNumber - 1) * request.PageSize} LIMIT {request.PageSize}");

            using (var connection = CreateConnection())
            {
                return (await connection.QueryAsync<User>(query.ToString(), prepare.Item2)).ToList();
            }
        }

        public async Task<List<User>> GetAllByUserName(Guid auid, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return null;

            var searchQuery = "%" + username + "%";

            // Old
            // var query = "SELECT * FROM \"Users\" WHERE \"UserName\" LIKE '%@UserName%' AND \"Id\" != @Id AND \"IsSoftDeleted\" = false ORDER BY \"CreatedAt\" DESC OFFSET @OffsetNumber LIMIT @Limit";    
            var query = "SELECT * FROM \"Users\" WHERE \"UserName\" LIKE @UserName AND \"Id\" != @Id AND \"IsSoftDeleted\" = false ORDER BY \"CreatedAt\" DESC";
            var parameters = new DynamicParameters();

            parameters.Add("UserName", searchQuery, DbType.String);
            parameters.Add("Id", auid, DbType.Guid);
            parameters.Add("IsSoftDeleted", false, DbType.Boolean);
            // parameters.Add("OffsetNumber", (request.PageSize - 1) * request.PageNumber, DbType.Int32);
            // parameters.Add("Limit", request.PageNumber, DbType.Int32);

            using (var connection = CreateConnection())
            {
                return (await connection.QueryAsync<User>(query.ToString(), parameters)).ToList();
            }
        }

        public async Task<User> GetByIdAsync<IdType>(IdType id)
        {
            if (id.GetType() != typeof(Guid))
                throw new Exception("The User Id type is not valid");

            var query = "SELECT * FROM \"Users\" WHERE \"Id\" = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);

            using (var connection = CreateConnection())
            {
                return (await connection.QueryFirstOrDefaultAsync<User>(query, parameters));
            }
        }
        public async Task<List<User>> GetListByIdAsync(List<Guid> ids)
        {
            if (null == ids || ids.Count == 0)
                throw new Exception("The List UserId is not valid");

            var query = "SELECT * FROM \"Users\" WHERE \"Id\"  = ANY(@Ids)";
            var parameters = new DynamicParameters();
            parameters.Add("Ids", ids);
            List<User> entities = null;
            using (var connection = CreateConnection())
            {
                entities = connection.Query<User>(query, parameters).AsList();
            }
            return entities;
        }
        public async Task<long> GetTotalUser(GetAllUserRequest request)
        {
            var prepare = PrepareGetAllQuery(request);
            var query = new StringBuilder("SELECT COUNT(*) FROM \"Users\"");
            query.Append(prepare.Item1);

            using (var connection = CreateConnection())
            {
                return (await connection.QueryFirstAsync<long>(query.ToString(), prepare.Item2));
            }
        }

        public async Task<int> UpdateAsync(User entity)
        {
            var query = "UPDATE \"Users\" SET \"FullName\" = @FullName, \"Email\" = @Email, \"Credit\" = @Credit, \"Status\" = @Status, \"IsSoftDeleted\" = @IsSoftDeleted, \"Coin\" = @Coin,\"UpdatedAt\" = @UpdatedAt WHERE \"Id\" = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", entity.Id, DbType.Guid);
            parameters.Add("FullName", entity.FullName, DbType.String);
            parameters.Add("Email", entity.Email, DbType.String);
            parameters.Add("Credit", entity.Credit, DbType.Int64);
            parameters.Add("Status", entity.Status, DbType.Int16);
            parameters.Add("IsSoftDeleted", entity.IsSoftDeleted, DbType.Boolean);
            parameters.Add("Coin", entity.Coin, DbType.Int64);
            parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);

            using (var connection = CreateConnection())
            {
                return (await connection.ExecuteAsync(query, parameters));
            }
        }

        public async Task<int> UpdateTypeAsync(User entity, bool isConfimCrator)
        {
            string query = "";
            var parameters = new DynamicParameters();
            if (isConfimCrator)
            {
                query = "UPDATE \"Users\" SET \"UserType\" = @UserType WHERE \"Id\" = @Id";
                parameters.Add("Id", entity.Id, DbType.Guid);
                parameters.Add("UserType", entity.UserType, DbType.Int16);
            }
            else
            {
                query = "UPDATE \"Users\" SET \"IsRequestCreator\" = @IsRequestCreator WHERE \"Id\" = @Id";
                parameters.Add("Id", entity.Id, DbType.Guid);
                parameters.Add("IsRequestCreator", entity.IsRequestCreator, DbType.Boolean);
            }
            using (var connection = CreateConnection())
            {
                return (await connection.ExecuteAsync(query, parameters));
            }

        }

        private (StringBuilder, DynamicParameters) PrepareGetAllQuery(GetAllUserRequest request)
        {
            var parameters = new DynamicParameters();
            StringBuilder query = new StringBuilder();
            bool isCombineFilter = false;
            if (!string.IsNullOrEmpty(request.Email) ||
                !string.IsNullOrEmpty(request.FullName) ||
                request.Status.HasValue ||
                request.DateFrom.HasValue ||
                request.DateTo.HasValue)
            {
                query.Append(" WHERE");
            }
            if (!string.IsNullOrEmpty(request.Email))
            {
                var emailQuery = $"%{request.Email}%";
                query.Append(" \"Email\" LIKE @Email");
                parameters.Add("Email", emailQuery, DbType.String);
                isCombineFilter = true;
            }
            if (!string.IsNullOrEmpty(request.FullName))
            {
                var fullNameQuery = $"%{request.FullName}%";
                if (isCombineFilter)
                    query.Append(" AND");
                query.Append(" \"FullName\" LIKE @FullName");
                parameters.Add("FullName", fullNameQuery, DbType.String);
                isCombineFilter = true;
            }
            if (request.Status.HasValue)
            {
                if (isCombineFilter)
                    query.Append(" AND");
                query.Append(" \"Status\" = @Status");
                parameters.Add("Status", request.Status.Value, DbType.Int16);
                isCombineFilter = true;
            }
            if (request.DateFrom.HasValue)
            {
                if (isCombineFilter)
                    query.Append(" AND");
                query.Append(" \"CreatedAt\" >= @DateFrom");
                parameters.Add("DateFrom", request.DateFrom.Value, DbType.DateTime2);
                isCombineFilter = true;
            }
            if (request.DateTo.HasValue)
            {
                if (isCombineFilter)
                    query.Append(" AND");
                query.Append(" \"CreatedAt\" <= @DateTo");
                parameters.Add("DateTo", request.DateTo.Value, DbType.DateTime2);
                isCombineFilter = true;
            }

            return (query, parameters);
        }

        public async Task<List<UserRequestFriendDto>> GetRequestFriend(Guid id)
        {
            var query = "SELECT DISTINCT uc1.\"UserId\" AS RequestUserId, u.\"FullName\" AS RequestFullName, ud.\"Avatar\" AS Avatar, u.\"UserType\" AS RequestType " +
                        "FROM \"UserCategories\" uc1 " +
                        "JOIN \"UserCategories\" uc2 ON uc1.\"CategoryId\" = uc2.\"CategoryId\" " +
                        "JOIN \"Users\" u ON u.\"Id\" = uc1.\"UserId\" " +
                        "LEFT JOIN \"UserDetails\" ud ON ud.\"UserId\" = u.\"Id\" " +
                        "WHERE u.\"Id\" != @Id " +
                        "AND NOT EXISTS " +
                                 "(SELECT 1 " +
                                 "FROM \"UserRequestFriendActions\" ur " +
                                 "WHERE ur.\"UserId\" = u.\"Id\" OR ur.\"RequestUserId\" = u.\"Id\")  " +
                        "AND uc2.\"UserId\" = @Id LIMIT 5;";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            try
            {
                using (var connection = CreateConnection())
                {
                    return (await connection.QueryAsync<UserRequestFriendDto>(query, parameters)).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting friend requests.", ex);
            }

        }

        public async Task<List<Guid>> GetCreatorsAsync(UserSimilarityRequest request)
        {
            var query = "SELECT DISTINCT U.\"Id\" " +
                        "FROM \"Users\" U " +
                        "JOIN \"UserCategories\" UC1 ON UC1.\"UserId\" = U.\"Id\" " +
                        "JOIN \"UserCategories\" UC2 ON UC2.\"CategoryId\" = UC1.\"CategoryId\" AND UC2.\"UserId\" = @UserId " +
                        "JOIN \"UserDetails\" UD ON UD.\"UserId\" = U.\"Id\" AND UD.\"IsFollower\" = @IsFollower " +
                        "WHERE U.\"Id\" != @UserId AND U.\"UserType\" = @UserType AND UD.\"Sex\" = @Sex AND SUBSTRING(UD.\"Dob\", 1, 4) = @Dob " +
                        "LIMIT 50";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", request.UserId, DbType.Guid);
            parameters.Add("IsFollower", false, DbType.Boolean);
            parameters.Add("Sex", request.Sex, DbType.String);
            parameters.Add("Dob", request.Dob, DbType.String);
            parameters.Add("UserType", request.UserType, DbType.Int32);
            using (var connection = CreateConnection())
            {
                return (await connection.QueryAsync<Guid>(query, parameters)).ToList();
            }

        }

    }
}
