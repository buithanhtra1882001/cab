using CabGroupService.Infrastructures.Repositories.GenericRepository;
using CabGroupService.Infrastructures.Repositories.Interfaces;
using CabGroupService.Models.Dtos;
using CabGroupService.Models.Dtos.GroupMembers;
using CabGroupService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CabGroupService.Infrastructures.Repositories
{
    public class GroupMemberRepository : GenericRepository<GroupMembers>, IGroupMemberRepository
    {
        public GroupMemberRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public bool IsUserInGroup(Guid groupId, Guid userId)
        {
            return _dbSet.Any(item => item.GroupID == groupId && item.UserID == userId);
        }

        public bool IsAdminUser(Guid groupId, Guid userId)
        {
            return _dbSet.Any(item => item.GroupID == groupId && item.UserID == userId && item.Permissions == Constants.GroupPermissions.ADMIN);
        }

        public async Task<ModelPagingResponse<GroupMembers>> GroupMemberPendingApprovalWithPagination(PagingGroupMemberRequest pagingRequest)
        {
            ModelPagingResponse<GroupMembers> data = new ModelPagingResponse<GroupMembers>();
            data.Data = await _dbSet.Where(x=>x.GroupID == pagingRequest.GroupID && x.Status == Constants.GroupMemberStatus.PENDING).OrderByDescending(x => x.CreatedAt).Skip((pagingRequest.PageNumber - 1) * pagingRequest.PageSize).Take(pagingRequest.PageSize).ToListAsync();
            data.Total = pagingRequest.PageSize == 0 ? 0 : (int)Math.Ceiling((double)_dbSet.Count(x => x.GroupID == pagingRequest.GroupID && x.Status == Constants.GroupMemberStatus.PENDING) / pagingRequest.PageSize);
            return data;
        }
    }
}
