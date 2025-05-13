using CabMediaService.Models.Dtos;

namespace CabMediaService.Services.Interfaces
{
    public interface IDropBoxMediaService
    {
        Task<List<UploadImageResponse>> UploadImageAsync(IFormFileCollection files, Guid userId);
        Task<List<UploadVideoResponse>> UploadVideoAsync(Guid userId, IFormFileCollection files, string bearerToken);

    }
}
