using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using System.Threading.Tasks;

namespace CabPostService.Infrastructures.Repositories.Interfaces;

public interface ISharePostRepository : IPostgresBaseRepository<SharePost>
{
  Task<IEnumerable<SharePost>> GetAllPostShareByFilterAsync(SharePostFilter sharePostFilter);

  Task<long> GetTotalAsync(SharePostFilter sharePostFilter);
}