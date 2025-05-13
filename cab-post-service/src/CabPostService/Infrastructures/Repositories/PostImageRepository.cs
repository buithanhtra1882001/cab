using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Repositories.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;
using Cassandra;

namespace CabPostService.Infrastructures.Repositories
{
    public class PostImageRepository :
        BaseRepository<PostImage>,
        IPostImageRepository
    {
        private readonly Cassandra.ISession _session;

        public PostImageRepository(ScyllaDbContext context)
            : base(context)
        {
            _session = context._session;
        }

        public async Task CreateManyAsync(IEnumerable<PostImage> entities)
        {
            if (entities is null || !entities.Any())
                throw new ArgumentNullException(nameof(entities));

            var preparedStatement = await _session.PrepareAsync("INSERT INTO post_posts_images (id, post_id, image_id, image_url, is_violence, created_at) VALUES (?, ?, ?, ?, ?, ?)");

            foreach (var item in entities)
            {
                try
                {

                    var boundStatement = preparedStatement.Bind(item.Id, item.PostId, item.ImageId, item.Url, item.IsViolence, item.CreatedAt);
                    await _session.ExecuteAsync(boundStatement);
                }
                catch (Exception ex)
                {
                }
            }
        }
        public async Task<List<PostImage>> GetPostImagesAsync(List<string> postIds)
        {
            var preparedStatement = await _session.PrepareAsync("SELECT * FROM post_posts_images WHERE post_id IN ? ");
            var batchStatement = preparedStatement.Bind(postIds);
            var rs = await _session.ExecuteAsync(batchStatement);
            var result = rs.Select(x => new PostImage
            {
                PostId = x.GetValue<string>("post_id"),
                ImageId = x.GetValue<Guid>("image_id"),
                Url = x.GetValue<string>("image_url"),
                IsViolence = x.GetValue<bool>("is_violence"),
                CreatedAt = x.GetValue<DateTime>("created_at"),
                Id = x.GetValue<Guid>("id")
            }).ToList();

            return result;
        }
        public async Task<PostImage> GetPostImageByIdAsync(Guid id)
        {
            var preparedStatement = await _session.PrepareAsync("SELECT * FROM post_posts_images WHERE image_id = ? ALLOW FILTERING");

            var batchStatement = preparedStatement.Bind(id);

            var resultSet = await _session.ExecuteAsync(batchStatement);

            var row = resultSet.FirstOrDefault();

            if (row == null)
                return null;

            return new PostImage
            {
                PostId = row.GetValue<string>("post_id"),
                ImageId = row.GetValue<Guid>("image_id"),
                Url = row.GetValue<string>("image_url"),
                IsViolence = row.GetValue<bool>("is_violence"),
                CreatedAt = row.GetValue<DateTime>("created_at"),
                Id = row.GetValue<Guid>("id")
            };
        }
    }
}