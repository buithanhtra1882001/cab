using CabPostService.Models.Dtos;

namespace CabPostService.Clients.Interfaces
{
    public interface IMediaClient
    {
        Task UpdateImagesAsync(string bearerToken, IEnumerable<Guid> guids);
        Task<IEnumerable<VideoUploadResponse>> UploadVideoAsync(string bearerToken, string type, IFormFileCollection files);
    }
}