using CabGroupService.Models.Dtos;
using CabGroupService.Models.Dtos.Group;
using CabGroupService.Models.Entities;

namespace CabGroupService.Services.Interfaces
{
    public interface IGroupService
    {
        Task<PagingResponse<GroupResponse>> GetGroupsWithPagination(PagingRequest pagingRequest);
        Task<GroupResponse> GetGroupDetail(Guid groupId);
        Task<bool> DeleteGroup(RequestDeleteGroup request);
        Task<Guid> AddGroup(RequestAddGroup request);
        Task<bool> UpdateGroup(Guid groupId, RequestUpdateGroup request);
    }
}
