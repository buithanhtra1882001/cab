using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using Dapper;
using System.Data;
using System.Text;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostNotifyAdminRepository :
        PostgresBaseRepository,
        IPostNotifyAdminRepository
    {
        private readonly ILogger<PostNotifyAdminRepository> _logger;

        public PostNotifyAdminRepository(
            IConfiguration configuration,
            ILogger<PostNotifyAdminRepository> logger
        ) : base(configuration)
        {
            _logger = logger;
        }

        public void InsertOne(PostNotifyAdmin model)
        {
            try
            {
                using var connection = CreateConnection();
                connection.Execute(@"
                    INSERT INTO ""PostNotifyAdmin"" (""Id"", ""PostId"", ""IsAcceptHide"", ""IsHandle"", ""IsRead"", ""IsDelete"", ""ReportNumber"", ""Description"", ""CreatedAt"") 
                    VALUES (
                    @Id,
                    @PostId,
                    @IsAcceptHide,
                    @IsHandle,
                    @IsRead,
                    @IsDelete,
                    @ReportNumber,
                    @Description               
                    )", param: model);
            }
            catch (Exception e)
            {
                _logger.LogError("InsertOne: " + e.Message);
                throw;
            }
        }

        public void UpdateIsAcceptHide(bool isAcceptHide, string idNotify)
        {
            try
            {
                using var connection = CreateConnection();
                var query = "UPDATE \"PostNotifyAdmin\" SET \"IsAcceptHide\" = @IsAcceptHide, \"IsHandle\" = @IsHandle, \"IsRead\" = @IsRead WHERE \"Id\" = @Id";
                
                var parameters = new DynamicParameters();
                parameters.Add("Id", idNotify);
                parameters.Add("IsAcceptHide", isAcceptHide);
                parameters.Add("IsHandle", true);
                parameters.Add("IsRead", true);
                connection.Execute(query, parameters);
            }
            catch (Exception e)
            {
                _logger.LogError("UpdateIsAcceptHide: " + e.Message);
                throw;
            }
        }
    }
}