using CabGroupService.Constants;
using CabGroupService.Models.Dtos;
using CabGroupService.Models.Dtos.Group;
using CabGroupService.Models.Dtos.GroupMembers;

namespace CabGroupService.Services.Interfaces
{
    public interface IGroupMemberService
    {
        Task<GroupMemberStatus> joinGroup(RequestJoinGroup request);
        Task<bool> leaveGroup(RequestLeaveGroup request);
        Task<bool> cancelRequest(RequestLeaveGroup request);
        Task<bool> updatePermissions(RequestUpdatePermissions request);
        Task<PagingResponse<GroupMemberPendingApproval>> listGroupMemberPendingApproval(PagingGroupMemberRequest pagingRequest);
        Task<bool> approvalParticipationRequest(RequestApproval request);
    }
}