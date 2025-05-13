using CabUserService.Infrastructures.Repositories.Base;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Entities;
using Dapper;
using System.Data;

namespace CabUserService.Infrastructures.Repositories
{
    public class CategoryRepository : PostgresBaseRepository, ICategoryRepository
    {
        public CategoryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Task<int> CreateAsync(Category entity)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var query = "SELECT * FROM \"Categories\" ORDER BY \"SortOrder\";";
            var parameters = new DynamicParameters();
            using (var connection = CreateConnection())
            {
                return (await connection.QueryAsync<Category>(query, parameters)).ToList();
            }
        }


        public async Task<Category> GetByIdAsync<IdType>(IdType id)
        {
            var query = "SELECT * FROM \"Categories\" WHERE \"Id\" = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);
            using (var connection = CreateConnection())
            {
                return (await connection.QueryFirstOrDefaultAsync<Category>(query, parameters));
            };
        }

        public Task<int> HardDeleteAsync<IdType>(IdType id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(Category entity)
        {
            throw new NotImplementedException();
        }
    }
}
