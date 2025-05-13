using CabGroupService.Constants;
using CabGroupService.Infrastructures.Repositories.Interfaces;
using CabGroupService.Infrastructures.Repositories;
using CabGroupService.Infrastructures;
using CabGroupService.Models.Dtos;
using CabGroupService.Services.BaseService;
using CabGroupService.Services.Interfaces;
using AutoMapper;
using CabGroupService.Models.Entities;
using CabGroupService.Models.Dtos.GroupMembers;
using CabGroupService.Models.Dtos.Group;
using MediatR;

namespace CabGroupService.Services
{
    public class GroupMemberService : BaseService<GroupMemberService>, IGroupMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMapper _mapper;
        public GroupMemberService(IMapper mapper, ILogger<GroupMemberService> logger) : base(logger)
        {
            _unitOfWork = new UnitOfWork();
            _groupMemberRepository = new GroupMemberRepository(_unitOfWork);
            _groupRepository = new GroupRepository(_unitOfWork);
            _mapper = mapper;
        }
        public async Task<GroupMemberStatus> joinGroup(RequestJoinGroup request)
        {
            try
            {
                Group group = await _groupRepository.GetByID(request.GroupID);
                if (group is null)
                    throw new NotImplementedException("Group not found!");

                bool isUserInGroup = _groupMemberRepository.IsUserInGroup(request.GroupID, request.UserID);
                if (isUserInGroup)
                    throw new NotImplementedException("The user is already a member of the group!");

                GroupMembers groupMembers = _mapper.Map<GroupMembers>(request);
                GroupMemberStatus status = group.ApprovalRequired ? GroupMemberStatus.PENDING : GroupMemberStatus.ACTIVE;
                groupMembers.Status = status;
                if (!group.ApprovalRequired)
                    groupMembers.JoinDate = DateTime.UtcNow;

                await _groupMemberRepository.Insert(groupMembers);
                _unitOfWork.Save();
                return status;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new NotImplementedException(e.Message);
            }
        }

        public async Task<bool> leaveGroup(RequestLeaveGroup request){
            try
            {
                Group group = await _groupRepository.GetByID(request.GroupID);
                if (group is null)
                    throw new NotImplementedException("Group not found!");

                bool isUserInGroup = _groupMemberRepository.IsUserInGroup(request.GroupID, request.UserID);
                if (!isUserInGroup)
                    throw new NotImplementedException("The user is not a member of the group!");

                GroupMembers? groupMembers = _groupMemberRepository.Get(x => x.GroupID == request.GroupID && x.UserID == request.UserID && x.Status == GroupMemberStatus.ACTIVE).FirstOrDefault();
                if (groupMembers is null)
                    return false;

                await _groupMemberRepository.Delete(groupMembers);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new NotImplementedException(e.Message);
            }
        }

        public async Task<bool> cancelRequest(RequestLeaveGroup request)
        {
            try
            {
                GroupMembers? groupMembers = _groupMemberRepository.Get(x => x.GroupID == request.GroupID && x.UserID == request.UserID && x.Status == GroupMemberStatus.PENDING).FirstOrDefault();
                if (groupMembers is null)
                    return false;

                await _groupMemberRepository.Delete(groupMembers);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new NotImplementedException(e.Message);
            }
        }

        public async Task<bool> updatePermissions(RequestUpdatePermissions request)
        {
            try
            {
                Group group = await _groupRepository.GetByID(request.GroupID);
                if (group is null)
                    throw new NotImplementedException("Group not found!");

                bool isUserInGroup = _groupMemberRepository.IsUserInGroup(request.GroupID, request.UserID);
                if (!isUserInGroup)
                    throw new NotImplementedException("The user is not a member of the group!");

                bool isAdminUser = _groupMemberRepository.IsAdminUser(request.GroupID, request.UserDecentralization);
                if (!isAdminUser)
                    throw new NotImplementedException("Accounts without admin access!");

                GroupMembers? groupMembers = _groupMemberRepository.Get(x => x.GroupID == request.GroupID && x.UserID == request.UserID && x.Status == GroupMemberStatus.ACTIVE).FirstOrDefault();
                if (groupMembers is null)
                    return false;

                groupMembers.Permissions = request.Permissions;
                groupMembers.DecentralizationUser = request.UserDecentralization;
                groupMembers.UpdatedAt = DateTime.UtcNow;
                await _groupMemberRepository.Update(groupMembers);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new NotImplementedException(e.Message);
            }
        }

        public async Task<PagingResponse<GroupMemberPendingApproval>> listGroupMemberPendingApproval(PagingGroupMemberRequest pagingRequest)
        {
            bool isAdminUser = _groupMemberRepository.IsAdminUser(pagingRequest.GroupID, pagingRequest.UserID);
            if (!isAdminUser)
                throw new NotImplementedException("Accounts without admin access!");

            PagingResponse<GroupMemberPendingApproval> response = new PagingResponse<GroupMemberPendingApproval>();
            ModelPagingResponse<GroupMembers> data = await _groupMemberRepository.GroupMemberPendingApprovalWithPagination(pagingRequest);
            response.PageSize = pagingRequest.PageSize;
            response.PageNumber = pagingRequest.PageNumber;
            response.Total = data.Total;
            response.Elements = _mapper.Map<IEnumerable<GroupMemberPendingApproval>>(data.Data).ToList();
            return response;
        }
        
        public async Task<bool> approvalParticipationRequest(RequestApproval request)
        {
            try
            {
                Group group = await _groupRepository.GetByID(request.GroupID);
                if (group is null)
                    throw new NotImplementedException("Group not found!");

                bool isAdminUser = _groupMemberRepository.IsAdminUser(request.GroupID, request.UserApproval);
                if (!isAdminUser)
                    throw new NotImplementedException("Accounts without admin access!");

                GroupMembers? groupMembers = _groupMemberRepository.Get(x => x.GroupID == request.GroupID && x.UserID == request.UserRequest && x.Status == GroupMemberStatus.PENDING).FirstOrDefault();
                if (groupMembers is null)
                    throw new NotImplementedException("Users who don't have a request to join the group!");

                if (request.Approval)
                {
                    groupMembers.Status = GroupMemberStatus.ACTIVE;
                    groupMembers.JoinDate = DateTime.UtcNow;
                    groupMembers.JoinMethod = JoinMethod.Requested;
                    await _groupMemberRepository.Update(groupMembers);
                    _unitOfWork.Save();
                    return true;
                }

                await _groupMemberRepository.Delete(groupMembers);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new NotImplementedException(e.Message);
            }
        }
    }
}
