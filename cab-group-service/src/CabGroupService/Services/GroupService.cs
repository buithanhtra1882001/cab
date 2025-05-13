using AutoMapper;
using CabGroupService.Infrastructures;
using CabGroupService.Infrastructures.Repositories;
using CabGroupService.Infrastructures.Repositories.Interfaces;
using CabGroupService.Models.Dtos;
using CabGroupService.Models.Dtos.Group;
using CabGroupService.Models.Entities;
using CabGroupService.Services.BaseService;
using CabGroupService.Services.Interfaces;
using Cassandra.Data.Linq;

namespace CabGroupService.Services
{
    public class GroupService : BaseService<GroupService>, IGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGroupRepository _groupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IMapper _mapper;
        public GroupService(IMapper mapper, ILogger<GroupService> logger) : base(logger)
        {
            _unitOfWork = new UnitOfWork();
            _groupRepository = new GroupRepository(_unitOfWork);
            _groupMemberRepository = new GroupMemberRepository(_unitOfWork);
            _mapper = mapper;
        }

        public async Task<PagingResponse<GroupResponse>> GetGroupsWithPagination(PagingRequest pagingRequest)
        {
            PagingResponse<GroupResponse> response = new PagingResponse<GroupResponse>();
            ModelPagingResponse <Group> data = await _groupRepository.GetGroupsWithPagination(pagingRequest);
            response.PageSize = pagingRequest.PageSize;
            response.PageNumber = pagingRequest.PageNumber;
            response.Total = data.Total;
            response.Elements = _mapper.Map<IEnumerable<GroupResponse>>(data.Data).ToList();
            return response;
        }

        public async Task<GroupResponse> GetGroupDetail(Guid groupId)
        {
            return _mapper.Map<GroupResponse>(await _groupRepository.GetByID(groupId));
        }

        public async Task<bool> DeleteGroup(RequestDeleteGroup request)
        {
            try
            {
                Group group = await _groupRepository.GetByID(request.GroupId);
                if(group is null)
                    throw new NotImplementedException("Group not found!");

                if (group.CreatedByUser != request.UserId)
                    throw new NotImplementedException("Users do not have deletion rights!");

                IEnumerable<GroupMembers> groupMembers = _groupMemberRepository.Get(x => x.GroupID == request.GroupId);
                foreach (var item in groupMembers)
                {
                    await _groupMemberRepository.Delete(item);
                }
                await _groupRepository.Delete(group);
                _unitOfWork.Save();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new NotImplementedException(e.Message);
            }
        }

        public async Task<Guid> AddGroup(RequestAddGroup request)
        {
            try
            {
                _unitOfWork.CreateTransaction();
                Group group = _mapper.Map<Group>(request);
                await _groupRepository.Insert(group);

                GroupMembers groupMembers = new GroupMembers();
                groupMembers.GroupID = group.Id;
                groupMembers.UserID = request.CreatedByUser;
                groupMembers.JoinMethod = Constants.JoinMethod.AutoJoined;
                groupMembers.JoinDate = DateTime.UtcNow;
                groupMembers.Permissions = Constants.GroupPermissions.ADMIN;
                groupMembers.Status = Constants.GroupMemberStatus.ACTIVE;
                await _groupMemberRepository.Insert(groupMembers);
                _unitOfWork.Save();
                _unitOfWork.Commit();
                return group.Id;
            }
            catch (Exception e)
            {
                _unitOfWork.Rollback();
                _logger.LogError(e.Message);
                throw new NotImplementedException(e.Message);
            }
        }

        public async Task<bool> UpdateGroup(Guid groupId, RequestUpdateGroup request)
        {
            try
            {
                Group group = await _groupRepository.GetByID(groupId);
                if (group == null)
                    return false;
                _mapper.Map(request, group);
                await _groupRepository.Update(group);
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
