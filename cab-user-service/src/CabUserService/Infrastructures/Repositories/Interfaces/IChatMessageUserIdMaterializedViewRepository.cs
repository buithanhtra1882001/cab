using CabUserService.Models.Entities;

namespace CabUserService.Infrastructures.Repositories.Interfaces
{
    public interface IChatMessageUserIdMaterializedViewRepository : IBaseRepository<ChatMessageUserIdMaterializedView>
    {
        Task<List<ChatMessageUserIdMaterializedView>> GetListByUserIdAsync(Guid userId);
    }
}
