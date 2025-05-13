using CabUserService.Models.Dtos;

namespace CabUserService.Clients.Interfaces
{
    public interface IMediaClient
    {
        Task<IEnumerable<ImageUploadResponse>> UploadImagesAsync(string bearerToken, string type, IFormFileCollection files);
        Task<IEnumerable<ImageUploadResponse>> UploadImageAsync(string bearerToken, string type, IFormFile files);
    }
}