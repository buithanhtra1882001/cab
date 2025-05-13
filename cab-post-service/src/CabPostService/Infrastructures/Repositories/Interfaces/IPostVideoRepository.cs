using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories.Interfaces
{
    public interface IPostVideoRepository
    {
        Task<int> CreateAsync(PostVideo entity);
        Task<string> UpdateAVTP(string postVideoId, double avtp, DateTime updateAt);
        Task<PostVideo> GetByIdAsync<IdType>(IdType id);
        Task<List<PostVideo>> GetByPostIdsAsync<IdType>(IdType postId);

    }
}

