using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using Dapper;
using System.Data;
using System.Text;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostCategoryRepository :
        PostgresBaseRepository,
        IPostCategoryRepository
    {
        private readonly ILogger<PostCategoryRepository> _logger;

        public PostCategoryRepository(
            IConfiguration configuration,
            ILogger<PostCategoryRepository> logger)
            : base(configuration)
        {
            _logger = logger;
        }

        public void InsertCategories(List<PostCategory> postCategories)
        {
            using var connection = CreateConnection();

            postCategories.ForEach((cat) =>
            {
                connection.Execute(@"
                    INSERT INTO ""PostCategories"" (""Id"", ""Slug"", ""Name"", ""Description"", ""Thumbnail"", ""Status"", ""UpdatedAt"", ""CreatedAt"", ""IsSoftDeleted"",""Score"")
                    VALUES(
                    @Id,
                    @Slug,
                    @Name,
                    @Description,
                    @Thumbnail,
                    @Status,
                    @UpdatedAt,
                    @CreatedAt,
                    @IsSoftDeleted,
                    @Score
                    )", param: cat);
            });
        }

        public void SeedDataCategoryAndCategoryType()
        {
            using var connection = CreateConnection();
            connection.Execute(@"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";
                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('f887bcd1-ce67-4288-804a-ffd629ffa21b','the-thao','Thể thao','Thể thao','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'f887bcd1-ce67-4288-804a-ffd629ffa21b',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'f887bcd1-ce67-4288-804a-ffd629ffa21b',2, now(), now());

                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('f8d887ec-2634-49a7-8050-994ab58849bd','giai-tri-tong-hop','Giải trí tổng hợp','Giải trí tổng hợp','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'f8d887ec-2634-49a7-8050-994ab58849bd',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'f8d887ec-2634-49a7-8050-994ab58849bd',2, now(), now());

                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('eeeef7bd-12a9-4c6b-9cd6-2ecff6b77c1f','cong-nghe','Công nghệ','Công nghệ','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'eeeef7bd-12a9-4c6b-9cd6-2ecff6b77c1f',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'eeeef7bd-12a9-4c6b-9cd6-2ecff6b77c1f',2, now(), now());

                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('e68077a6-81f7-4f38-b0bf-2bbca9f45c05','thu-cung','Thú cưng','Thú cưng','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'e68077a6-81f7-4f38-b0bf-2bbca9f45c05',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'e68077a6-81f7-4f38-b0bf-2bbca9f45c05',2, now(), now());


                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('68a0ebe4-7188-41fe-9298-ae9f60af4e83','game','Game','Game','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'68a0ebe4-7188-41fe-9298-ae9f60af4e83',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'68a0ebe4-7188-41fe-9298-ae9f60af4e83',2, now(), now());


                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('e207679d-027d-4501-9f7c-ecefe885341b','phim','Phim','Phim','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'e207679d-027d-4501-9f7c-ecefe885341b',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'e207679d-027d-4501-9f7c-ecefe885341b',2, now(), now());

                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('fb4775f9-4d5d-4cc5-98e5-78345350cd1e','sang-tao','Sáng tạo','Sáng tạo','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'fb4775f9-4d5d-4cc5-98e5-78345350cd1e',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'fb4775f9-4d5d-4cc5-98e5-78345350cd1e',2, now(), now());

                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('efe1fd53-9129-4668-85d1-62ed3f478f67','nguoi-di-lam','Chủ đề người đi làm','Chủ đề người đi làm','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'efe1fd53-9129-4668-85d1-62ed3f478f67',1, now(), now());


                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('f9f1cff8-91b8-4151-9ce4-cab744311257','o-nha','Chủ đề ở nhà','Chủ đề ở nhà','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'f9f1cff8-91b8-4151-9ce4-cab744311257',1, now(), now());

                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('49528e7d-ab2e-4cd8-9bda-c10ed75cb1ad','nsfw','NSFW','NSFW','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'49528e7d-ab2e-4cd8-9bda-c10ed75cb1ad',1, now(), now());


                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('17955cd0-4bf8-47ef-b9eb-e90a772fd4e5','real-life','Real life','Real life','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'17955cd0-4bf8-47ef-b9eb-e90a772fd4e5',1, now(), now());


                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('9a7554e8-c14e-4cc8-984b-9736b60275b8','anime-wibu','Anime - wibu','Anime - wibu','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'9a7554e8-c14e-4cc8-984b-9736b60275b8',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'9a7554e8-c14e-4cc8-984b-9736b60275b8',2, now(), now());


                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('ef8e0ebf-a2e3-484f-8e1a-d1353183ca07','Food','Food','Food','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'ef8e0ebf-a2e3-484f-8e1a-d1353183ca07',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'ef8e0ebf-a2e3-484f-8e1a-d1353183ca07',2, now(), now());


                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('c62233be-8854-49f0-aeea-17991987ae2e','lua-tuoi','Lứa tuổi','Lứa tuổi','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'c62233be-8854-49f0-aeea-17991987ae2e',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'c62233be-8854-49f0-aeea-17991987ae2e',2, now(), now());

                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('68821426-c441-406e-9773-9ad57a051293','sinh-vien','Sinh viên','Sinh viên','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'68821426-c441-406e-9773-9ad57a051293',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'68821426-c441-406e-9773-9ad57a051293',2, now(), now());


                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('d5d559b2-f5fd-426e-9309-fcb27d160d95','hoc-sinh','Học sinh','Học sinh','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'d5d559b2-f5fd-426e-9309-fcb27d160d95',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'d5d559b2-f5fd-426e-9309-fcb27d160d95',2, now(), now());

                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('5ee09e3d-6c59-459a-a338-a2f28fa2b27b','nguoi-25','Người >25','Người >25','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'5ee09e3d-6c59-459a-a338-a2f28fa2b27b',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'5ee09e3d-6c59-459a-a338-a2f28fa2b27b',2, now(), now());

                                INSERT INTO ""PostCategories""(""Id"",""Slug"",""Name"",""Description"",""Thumbnail"",""Status"",""IsSoftDeleted"",""CreatedAt"",""UpdatedAt"",""Score"") 
                                VALUES ('cd093f98-dfde-446d-bf8c-3db03cc05c16','khong-gioi-han','Không giới hạn','Không giới hạn','https://loremflickr.com/320/240?lock=37395796', 1, false, now(), now(),2);
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'cd093f98-dfde-446d-bf8c-3db03cc05c16',1, now(), now());
                                INSERT INTO ""PostCategoryType""(""Id"",""PostCategoryId"",""Type"",""CreatedAt"",""UpdatedAt"")
                                VALUES (uuid_generate_v1(),'cd093f98-dfde-446d-bf8c-3db03cc05c16',2, now(), now());");
        }

        public async Task<int> CountPostCategory()
        {
            using var con = CreateConnection();

            return await con.ExecuteScalarAsync<int>(@"SELECT COUNT(*) FROM ""PostCategories""");
        }

        public async Task<int> DeleteAsync<IdType>(IdType id)
        {
            if (id == null || id.GetType() != typeof(string))
                throw new Exception("The Post Category Id Type is not valid");

            var query = "DELETE FROM \"PostCategories\" WHERE \"Id\" = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.String);

            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query, parameters);
        }

        public async Task<List<PostCategory>> GetAllAsync()
        {
            var query = new StringBuilder("SELECT * FROM \"PostCategories\" ORDER BY \"Position\"");

            using var connection = CreateConnection();
            return (await connection.QueryAsync<PostCategory>(query.ToString())).ToList();
        }

        public async Task<List<PostCategory>> GetByType(int Type)
        {
            var query = new StringBuilder("SELECT pc.* FROM \"PostCategories\" pc INNER JOIN \"PostCategoryType\" pct ON pc.\"Id\" = pct.\"PostCategoryId\" WHERE pct.Type = @Type  ");

            var parameters = new DynamicParameters();
            parameters.Add("Type", Type, DbType.Int32);

            using var connection = CreateConnection();
            return (await connection.QueryAsync<PostCategory>(query.ToString(), parameters)).ToList();
        }

        public async Task<List<PostCategory>> GetAllAsync(GetAllPostCategoryFilter filter)
        {
            var prepare = PrepareGetAllQuery(filter);
            var query = new StringBuilder("SELECT * FROM \"PostCategories\"");

            var offset = (filter.PageNumber - 1) * filter.PageSize;

            query.Append(prepare.Item1);
            query.Append($" OFFSET {offset} LIMIT {filter.PageSize}");

            using var connection = CreateConnection();
            return (await connection.QueryAsync<PostCategory>(query.ToString(), prepare.Item2)).ToList();
        }

        public async Task<PostCategory> GetByIdAsync<IdType>(IdType id)
        {
            if (id == null || id.GetType() != typeof(Guid))
                throw new Exception("The Post Category Id Type is not valid");

            var query = "SELECT * FROM \"PostCategories\" WHERE \"Id\" = @Id";
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PostCategory>(query, parameters);
        }

        public async Task<PostCategory> GetBySlug(string slug)
        {
            var query = new StringBuilder("SELECT * FROM \"PostCategories\" WHERE \"Slug\" = @Slug");
            var parameters = new DynamicParameters();

            parameters.Add("Slug", slug, DbType.String);

            using var connection = CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<PostCategory>(query.ToString(), parameters);
        }

        public async Task<int> UpdateAsync(PostCategory entity)
        {
            var query = "UPDATE \"PostCategories\" SET \"Name\" = @Name, \"Description\" = @Description, \"Thumbnail\" = @Thumbnail, \"Status\" = @Status, \"UpdatedAt\" = @UpdatedAt WHERE \"Id\" = @Id";

            var parameters = new DynamicParameters();

            parameters.Add("Id", entity.Id, DbType.String);
            parameters.Add("Slug", entity.Slug, DbType.String);
            parameters.Add("Name", entity.Name, DbType.String);
            parameters.Add("Description", entity.Description, DbType.String);
            parameters.Add("Thumbnail", entity.Thumbnail, DbType.String);
            parameters.Add("Status", entity.Status, DbType.Int32);
            parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);

            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query, parameters);
        }

        public async Task<int> CreateAsync(PostCategory entity)
        {
            var query = "INSERT INTO \"PostCategories\" (\"Id\", \"Slug\", \"Name\", \"Description\", \"Thumbnail\", \"Status\", \"UpdatedAt\", \"CreatedAt\") VALUES(@Id, @Slug, @Name, @Description, @Thumbnail, @Status, @UpdatedAt, @CreatedAt)";
            var parameters = new DynamicParameters();

            parameters.Add("Id", entity.Id, DbType.String);
            parameters.Add("Slug", entity.Slug, DbType.String);
            parameters.Add("Name", entity.Name, DbType.String);
            parameters.Add("Description", entity.Description, DbType.String);
            parameters.Add("Thumbnail", entity.Thumbnail, DbType.String);
            parameters.Add("Status", entity.Status, DbType.Int32);
            parameters.Add("UpdatedAt", entity.UpdatedAt, DbType.DateTime2);
            parameters.Add("CreatedAt", entity.CreatedAt, DbType.DateTime2);

            using var connection = CreateConnection();
            return await connection.ExecuteAsync(query, parameters);
        }

        public async Task<long> GetTotalAsync(GetAllPostCategoryFilter filter)
        {
            var prepare = PrepareGetAllQuery(filter);
            var query = new StringBuilder("SELECT COUNT(*) FROM \"PostCategories\"");
            query.Append(prepare.Item1);

            using var connection = CreateConnection();
            return await connection.QueryFirstAsync<long>(query.ToString(), prepare.Item2);
        }

        private (StringBuilder, DynamicParameters) PrepareGetAllQuery(GetAllPostCategoryFilter filter)
        {
            var parameters = new DynamicParameters();
            StringBuilder query = new StringBuilder();
            bool isCombineFilter = false;

            if (filter.Status.HasValue)
                query.Append(" WHERE");

            if (filter.Status.HasValue)
            {
                if (isCombineFilter)
                    query.Append(" AND");

                query.Append(" WHERE \"Status\" = @Status");
                parameters.Add("Status", filter.Status.Value, DbType.Int32);
                isCombineFilter = true;
            }

            return (query, parameters);
        }
    }
}