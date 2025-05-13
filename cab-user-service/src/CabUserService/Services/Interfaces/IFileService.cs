using CabUserService.Models.Dtos;

namespace CabUserService.Services.Interfaces
{
    public interface IFileService
    {
        Task<IEnumerable<ImageUploadResponse>> UploadUserImagesAsync(string bearerToken, string type, Guid auid, IFormFileCollection files);
        Task<IEnumerable<ImageUploadResponse>> UploadAvatarUserImagesAsync(string bearerToken, string type, Guid auid, IFormFile files);
        Task<IEnumerable<ImageUploadResponse>> UploadBackgroundUserImagesAsync(string bearerToken, string type, Guid auid, IFormFile files);
    }
}