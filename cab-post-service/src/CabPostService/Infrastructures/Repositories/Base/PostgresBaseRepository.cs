using Npgsql;
using System.Data;

namespace CabPostService.Infrastructures.Repositories.Base
{
    public class PostgresBaseRepository
    {
        private readonly IConfiguration _configuration;

        protected PostgresBaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_configuration.GetValue<string>("PostDbConnectionString"));
        }
    }
}
