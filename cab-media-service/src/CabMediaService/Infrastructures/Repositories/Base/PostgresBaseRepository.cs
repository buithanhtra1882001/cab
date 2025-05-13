using Npgsql;
using System.Data;

namespace CabMediaService.Infrastructures.Repositories.Base
{
    public class PostgresBaseRepository
    {
        private readonly IConfiguration _configuration;

        protected PostgresBaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_configuration.GetValue<string>("MediaDbConnectionString"));
        }
    }
}
