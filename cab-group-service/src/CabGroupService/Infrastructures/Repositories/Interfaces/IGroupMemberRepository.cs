using CabGroupService.Infrastructures.Repositories.GenericRepository;
using CabGroupService.Models.Dtos;
using CabGroupService.Models.Dtos.GroupMembers;
using CabGroupService.Models.Entities;

namespace CabGroupService.Infrastructures.Repositories.Interfaces
{
    public interface IGroupMemberRepository: IGenericRepository<GroupMembers>
    {
        bool IsUserInGroup(Guid groupId, Guid userId);
        bool IsAdminUser(Guid groupId, Guid userId);
        Task<ModelPagingResponse<GroupMembers>> GroupMemberPendingApprovalWithPagination(PagingGroupMemberRequest pagingRequest);
    }
}
