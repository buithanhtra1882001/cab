using CabMediaService.Models.Dtos;

namespace CabMediaService.Services.Interfaces
{
    public interface IAWSMediaService
    {
        Task<MediaImageResponse> GetAsync(Guid id);
        Task<IEnumerable<MediaImageResponse>> GetListAsync(IEnumerable<Guid> ids);
        Task<IEnumerable<UploadImageResponse>> UploadAsync(Guid uuid, string type, IFormFileCollection files);
        Task<IEnumerable<UploadVideoResponse>> UploadVideosAsync(Guid uuid, string type, IFormFileCollection files, string bearerToken);
        Task UpdateLastUsedAsync(IEnumerable<Guid> guids, DateTime lastUsedAt);
        Task DeleteAsync(Guid uuid, Guid id);
        Task DeleteManyAsync(Guid uuid, IEnumerable<Guid> ids);
        Task<int> CleanAsync();
    }
}