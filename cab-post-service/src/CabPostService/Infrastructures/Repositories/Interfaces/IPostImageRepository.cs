using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories.Interfaces
{
    public interface IPostImageRepository : IBaseRepository<PostImage>
    {
        Task CreateManyAsync(IEnumerable<PostImage> entities);
        Task<List<PostImage>> GetPostImagesAsync(List<string> postIds);
        Task<PostImage> GetPostImageByIdAsync(Guid id);
    }
}