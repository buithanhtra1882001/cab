using CabUserService.Infrastructures.Repositories.Base;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Entities;
using Dapper;
using System.Data;

namespace CabUserService.Infrastructures.Repositories
{
    public class UserCategoryRepository : PostgresBaseRepository, IUserCategoryRepository
    {
        public UserCategoryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<int> CreateAsync(UserCategory entity)
        {
            var query = "INSERT INTO \"UserCategories\"(\"Id\", \"UserId\", \"CategoryId\", \"UpdatedAt\", \"CreatedAt\") VALUES(@Id, @UserId, @CategoryId, @UpdatedAt, @CreatedAt)";
            var parameters = new DynamicParameters();
            parameters.Add("Id", entity.Id, DbType.Guid);
            parameters.Add("UserId", entity.UserId, DbType.Guid);
            parameters.Add("CategoryId", entity.CategoryId, DbType.Guid);
            parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);
            parameters.Add("CreatedAt", entity.CreatedAt, DbType.DateTime2);
            using (var connection = CreateConnection())
            {
                return (await connection.ExecuteAsync(query, parameters));
            }
        }
        public async Task<List<Category>> GetUserFollowedCategoriesAsync(Guid userId)
        {
            var query = "SELECT * FROM \"UserCategories\" UC INNER JOIN \"Categories\" C ON UC.\"CategoryId\" = C.\"Id\" WHERE UC.\"UserId\" = @UserId";
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId, DbType.Guid);
            using (var connection = CreateConnection())
            {
                return (await connection.QueryAsync<Category>(query, parameters)).ToList();
            }
        }

        public async Task<UserCategory> GetUserCategoryByIdAsync(Guid userId, Guid categoryId)
        {
            var query = "SELECT * FROM \"UserCategories\" WHERE \"UserId\" = @UserId AND \"CategoryId\" = @CategoryId";
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId, DbType.Guid);
            parameters.Add("CategoryId", categoryId, DbType.Guid);
            using (var connection = CreateConnection())
            {
                return (await connection.QueryFirstOrDefaultAsync<UserCategory>(query, parameters));
            }
        }

        public Task<UserCategory> GetByIdAsync<IdType>(IdType id)
        {
            throw new NotImplementedException();
        }

        public Task<int> HardDeleteAsync<IdType>(IdType id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(UserCategory entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> AddRangeUserCategoryAsync(List<UserCategory> entities)
        {
            int rowEffects = 0;
            foreach (var entity in entities)
            {
                var query = "INSERT INTO \"UserCategories\"(\"Id\", \"UserId\", \"CategoryId\", \"UpdatedAt\", \"CreatedAt\") VALUES(@Id, @UserId, @CategoryId, @UpdatedAt, @CreatedAt)";
                var parameters = new DynamicParameters();
                parameters.Add("Id", entity.Id, DbType.Guid);
                parameters.Add("UserId", entity.UserId, DbType.Guid);
                parameters.Add("CategoryId", entity.CategoryId, DbType.Guid);
                parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);
                parameters.Add("CreatedAt", entity.CreatedAt, DbType.DateTime2);

                using (var connection = CreateConnection())
                {
                    await connection.ExecuteAsync(query, parameters);
                }
                rowEffects ++;
            }
            return rowEffects;

        }
    }
}
