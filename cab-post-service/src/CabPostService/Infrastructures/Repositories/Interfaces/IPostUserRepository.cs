using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories.Interfaces
{
    public interface IPostUserRepository
    {
        Task<bool> CheckExist(string postId, Guid userId);
        Task<int> CreateAsync(PostUsers entity);
        Task<int> UpdateAsync(PostUsers entity);
        Task<PostUsers> GetByUserIdAndPostId(Guid userId, string postId);
    }
}
