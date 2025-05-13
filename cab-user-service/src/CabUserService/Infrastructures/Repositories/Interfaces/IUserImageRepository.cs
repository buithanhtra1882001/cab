using CabUserService.Models.Entities;

namespace CabUserService.Infrastructures.Repositories.Interfaces
{
    public interface IUserImageRepository
    {
        public Task CreateAsync(IEnumerable<UserImage> userImages);
        public Task<UserImage> GetByIdAsync<IdType>(IdType id);
        public Task<List<UserImage>> GetListByUserIdAsync(List<Guid> ids);
    }
}
