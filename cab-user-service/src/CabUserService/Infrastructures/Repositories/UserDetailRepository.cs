using CabUserService.Infrastructures.Repositories.Base;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Entities;
using Dapper;
using System.Data;

namespace CabUserService.Infrastructures.Repositories
{
    public class UserDetailRepository : PostgresBaseRepository, IUserDetailRepository
    {
        public UserDetailRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> CreateAsync(UserDetail entity)
        {
            var query = @"INSERT INTO public.""UserDetails""(""UserDetailId"", ""UserId"", ""Dob"", ""Phone"", ""City"", ""Avatar"", ""CoverImage"", ""IdentityCardNumber"", ""Sex"", ""Description"", ""UpdatedAt"", ""CreatedAt"") VALUES(@UserDetailId, @UserId, @Dob, @Phone, @City, @Avatar, @CoverImage, @IdentityCardNumber, @Sex, @Description, @UpdatedAt, @CreatedAt)";
            var parameters = new DynamicParameters();
            parameters.Add("UserDetailId", entity.UserDetailId, DbType.Guid);
            parameters.Add("UserId", entity.UserId, DbType.Guid);
            parameters.Add("Dob", entity.Dob, DbType.String);
            parameters.Add("Phone", entity.Phone, DbType.String);
            parameters.Add("City", entity.City, DbType.String);
            parameters.Add("Avatar", entity.Avatar, DbType.String);
            parameters.Add("CoverImage", entity.CoverImage, DbType.String);
            parameters.Add("IdentityCardNumber", entity.IdentityCardNumber, DbType.String);
            parameters.Add("Sex", entity.Sex, DbType.String);
            parameters.Add("Description", entity.Description, DbType.String);
            parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);
            parameters.Add("CreatedAt", entity.CreatedAt, DbType.DateTime2);
            using (var connection = CreateConnection())
            {
                return (await connection.ExecuteAsync(query, parameters));
            }
        }

        /// <summary>
        /// Only used for unit test cleanup. Use soft delete in production work
        /// </summary>
        /// <typeparam name="IdType"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> HardDeleteAsync<IdType>(IdType id)
        {
            var query = @"DELETE FROM public.""UserDetails"" WHERE ""UserDetailId"" = @UserDetailId ";
            var parameters = new DynamicParameters();

            parameters.Add("UserDetailId", id, DbType.Guid);

            using var connection = CreateConnection();

            return await connection.ExecuteAsync(query, parameters);
        }

        public async Task<UserDetail> GetByIdAsync<IdType>(IdType id)
        {
            if (id.GetType() != typeof(Guid))
            {
                throw new Exception("The UserDetailId type is not valid");
            }
            var query = @"SELECT * FROM ""UserDetails"" WHERE ""UserId"" = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);

            using (var connection = CreateConnection())
            {
                return (await connection.QueryFirstOrDefaultAsync<UserDetail>(query, parameters));
            }
        }
        public async Task<List<UserDetail>> GetListDetailByIdAsync(List<Guid> ids)
        {
            if (null == ids || ids.Count == 0)
            {
                throw new Exception("The UserDetailIds type is not valid");
            }
            var query = @"SELECT * FROM ""UserDetails"" WHERE ""UserId"" = ANY(@Ids)";
            var parameters = new DynamicParameters();
            parameters.Add("Ids", ids);

            List<UserDetail> entities = null;
            using (var connection = CreateConnection())
            {
                entities = connection.Query<UserDetail>(query, parameters).AsList();
            }
            return entities;
        }

        public async Task<UserDetail> GetUserDetailByUserIdAsync(Guid userId)
        {
            var query = @"SELECT * FROM ""UserDetails"" WHERE ""UserId"" = @userId";
            var parameters = new DynamicParameters();
            parameters.Add("userId", userId, DbType.Guid);

            using (var connection = CreateConnection())
            {
                return (await connection.QueryFirstOrDefaultAsync<UserDetail>(query, parameters));
            }
        }

        public async Task<int> UpdateAsync(UserDetail entity)
        {
            var query = @"UPDATE public.""UserDetails"" SET ""UserId""=@UserId, ""Dob""=@Dob, ""Phone""=@Phone, ""City""=@City, ""Avatar""=@Avatar, ""CoverImage""=@CoverImage, ""IdentityCardNumber""=@IdentityCardNumber, ""Sex""=@Sex, ""Description""=@Description, ""IsUpdateProfile""=@IsUpdateProfile, ""UpdatedAt""=@UpdatedAt, ""CreatedAt""=@CreatedAt WHERE ""UserDetailId""=@UserDetailId";
            var parameters = new DynamicParameters();
            parameters.Add("UserId", entity.UserId, DbType.Guid);
            parameters.Add("UserDetailId", entity.UserDetailId, DbType.Guid);
            parameters.Add("Dob", entity.Dob, DbType.String);
            parameters.Add("Phone", entity.Phone, DbType.String);
            parameters.Add("City", entity.City, DbType.String);
            parameters.Add("Avatar", entity.Avatar, DbType.String);
            parameters.Add("CoverImage", entity.CoverImage, DbType.String);
            parameters.Add("IdentityCardNumber", entity.IdentityCardNumber, DbType.String);
            parameters.Add("Sex", entity.Sex, DbType.String);
            parameters.Add("Description", entity.Description, DbType.String);
            parameters.Add("IsUpdateProfile", entity.IsUpdateProfile, DbType.Boolean);
            parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);
            parameters.Add("CreatedAt", entity.CreatedAt, DbType.DateTime2);

            using (var connection = CreateConnection())
            {
                return (await connection.ExecuteAsync(query, parameters));
            }
        }

        public async Task<int> UpdateFollowerAsync(Guid UserId, string follower)
        {
            var query = @"UPDATE public.""UserDetails"" SET ""Follower""=@Follower WHERE ""UserId""=@UserId";
            var parameters = new DynamicParameters();
            parameters.Add("UserId", UserId, DbType.Guid);
            parameters.Add("Follower", follower, DbType.String);

            using (var connection = CreateConnection())
            {
                return (await connection.ExecuteAsync(query, parameters));
            }
        }

        public async Task<int> UpdateFollowingAsync(Guid UserId, string following)
        {
            var query = @"UPDATE public.""UserDetails"" SET ""Following""=@Following WHERE ""UserId""=@UserId";
            var parameters = new DynamicParameters();
            parameters.Add("UserId", UserId, DbType.Guid);
            parameters.Add("Following", following, DbType.String);

            using (var connection = CreateConnection())
            {
                return (await connection.ExecuteAsync(query, parameters));
            }
        }


    }
}