using CabMediaService.Models.Dtos;
using CabMediaService.Models.Entities;

namespace CabMediaService.Infrastructures.Repositories.Interfaces
{
    public interface IMediaImageRepository : IBaseRepository<MediaImage>
    {
        Task CreateManyAsync(IEnumerable<MediaImage> entities);
        Task<IEnumerable<UploadImageResponse>> UploadFileAsync(Guid uuid, string type, IFormFileCollection files);
    }
}