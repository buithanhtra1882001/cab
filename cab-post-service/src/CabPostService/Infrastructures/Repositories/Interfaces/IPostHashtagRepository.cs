using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;

namespace CabPostService.Infrastructures.Repositories.Interfaces
{
    public interface IPostHashtagRepository : IPostgresBaseRepository<PostHashtag>
    {
        Task<List<PostHashtag>> GetByName(string name);
        Task UpdateMultipleAsync(List<PostHashtag> list);
        Task<List<PostHashtag>> GetDataByLimit(int limit);
        Task<List<PostHashtag>> SearchDataBySlug(string slug);
    }
}