using CabGroupService.Infrastructures.Repositories.GenericRepository;
using CabGroupService.Models.Dtos;
using CabGroupService.Models.Entities;

namespace CabGroupService.Infrastructures.Repositories.Interfaces
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        Task<ModelPagingResponse<Group>> GetGroupsWithPagination(PagingRequest pagingRequest);
    }
}
