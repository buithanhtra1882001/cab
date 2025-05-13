using CabUserService.Infrastructures.DbContexts;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.Models.Entities;
using Cassandra;

namespace CabUserService.Infrastructures.Repositories
{
    public class UserImageRepository : IUserImageRepository
    {
        private readonly Cassandra.ISession _session;
        public UserImageRepository(ScyllaDbContext context)
        {
            _session = context._session;
        }

        public async Task CreateAsync(IEnumerable<UserImage> userImages)
        {
            var preparedStatement = await _session.PrepareAsync("INSERT INTO user_images (user_id, url, size, created_at, updated_at) VALUES (?, ?, ?, ?, ?)");
            var batchStatement = new BatchStatement();
            foreach (var item in userImages)
            {
                batchStatement.Add(preparedStatement.Bind(item.UserId, item.Url, item.Size, item.CreatedAt, item.UpdatedAt));
            }
            await _session.ExecuteAsync(batchStatement);
        }


        public async Task<UserImage> GetByIdAsync<IdType>(IdType id)
        {
            if (id.GetType() != typeof(Guid))
            {
                throw new Exception("The User Image Id type is not valid");
            }

            var preparedStatement = await _session.PrepareAsync("SELECT * FROM user_images WHERE user_id = ?");
            var statement = preparedStatement.Bind(id);

            var resultSet = await _session.ExecuteAsync(statement);
            var row = resultSet.FirstOrDefault();

            return row != null ? new UserImage
            {
                UserId = row.GetValue<Guid>("user_id"),
                Url = row.GetValue<string>("url"),
                Size = row.GetValue<double>("size"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                UpdatedAt = row.GetValue<DateTime>("updated_at")
            } : null;
        }
        public async Task<List<UserImage>> GetListByUserIdAsync(List<Guid> ids)
        {
            if (null == ids || ids.Count == 0)
                throw new Exception("The List UserId is not valid");

            var preparedStatement = await _session.PrepareAsync("SELECT * FROM user_images WHERE user_id IN ?");
            var statement = preparedStatement.Bind(ids);

            var resultSet = await _session.ExecuteAsync(statement);

            // Tạo danh sách kết quả
            var userImages = new List<UserImage>();

            foreach (var row in resultSet)
            {
                userImages.Add(new UserImage
                {
                    UserId = row.GetValue<Guid>("user_id"),
                    Url = row.GetValue<string>("url"),
                });
            }
            return userImages;
        }
    }
}
