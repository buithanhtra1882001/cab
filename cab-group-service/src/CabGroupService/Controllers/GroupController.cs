using CabGroupService.Models.Dtos;
using CabGroupService.Models.Dtos.Group;
using CabGroupService.Models.Dtos.GroupMembers;
using CabGroupService.Services;
using CabGroupService.Services.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace CabGroupService.Controllers
{
    [Route("api/v1/group")]
    [ApiController]
    [EnableCors("CabCors")]
    public class GroupController : ApiController<GroupController>
    {
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;
        private ResultResponse _result;
        public GroupController(IGroupService groupService, IGroupMemberService groupMemberService, ILogger<GroupController> logger) : base(logger)
        {
            _groupService = groupService;
            _groupMemberService = groupMemberService;
            _result = new ResultResponse();
        }

        [HttpGet]
        [Route("get-list-group")]
        public async Task<IActionResult> GetGroupsWithPagination([FromQuery] PagingRequest pagingRequest)
        {
            try
            {
                _result.Data = await _groupService.GetGroupsWithPagination(pagingRequest);
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            return Ok(_result);
        }

        [HttpGet]
        [Route("get-group-detail")]
        public async Task<IActionResult> GetGroupDetail([FromQuery] Guid groupId)
        {
            try
            {
                _result.Data = await _groupService.GetGroupDetail(groupId);
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            return Ok(_result);
        }

        [HttpPost]
        [Route("delete-group")]
        public async Task<IActionResult> DeleteGroup([FromBody] RequestDeleteGroup request)
        {
            try
            {
                _result.Data = await _groupService.DeleteGroup(request);
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            return Ok(_result);
        }

        [HttpPost]
        [Route("add-group")]
        public async Task<IActionResult> AddGroup([FromBody] RequestAddGroup request)
        {
            try
            {
                _result.Data = await _groupService.AddGroup(request);
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            return Ok(_result);
        }


        [HttpPost]
        [Route("update-group")]
        public async Task<IActionResult> UpdateGroup([FromQuery] Guid groupId, [FromBody] RequestUpdateGroup request)
        {
            try
            {
                _result.Data = await _groupService.UpdateGroup(groupId, request);
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            return Ok(_result);
        }


        [HttpPost]
        [Route("join-group")]
        public async Task<IActionResult> JoinGroup([FromBody] RequestJoinGroup request)
        {
            try
            {
                _result.Data = await _groupMemberService.joinGroup(request);
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            return Ok(_result);
        }

        [HttpPost]
        [Route("leave-group")]
        public async Task<IActionResult> leaveGroup([FromBody] RequestLeaveGroup request)
        {
            try
            {
                _result.Data = await _groupMemberService.leaveGroup(request);
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            return Ok(_result);
        }

        [HttpPost]
        [Route("cancel-request")]
        public async Task<IActionResult> cancelRequest([FromBody] RequestLeaveGroup request)
        {
            try
            {
                _result.Data = await _groupMemberService.cancelRequest(request);
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            return Ok(_result);
        }

        [HttpPost]
        [Route("update-permission")]
        public async Task<IActionResult> updatePermissions([FromBody] RequestUpdatePermissions request)
        {
            try
            {
                _result.Data = await _groupMemberService.updatePermissions(request);
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            return Ok(_result);
        }

        [HttpGet]
        [Route("list-group-member-pending-approval")]
        public async Task<IActionResult> listGroupMemberPendingApproval([FromQuery] PagingGroupMemberRequest request)
        {
            try
            {
                _result.Data = await _groupMemberService.listGroupMemberPendingApproval(request);
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            return Ok(_result);
        }


        [HttpPost]
        [Route("approval-participation-request")]
        public async Task<IActionResult> approvalParticipationRequest([FromBody] RequestApproval request)
        {
            try
            {
                _result.Data = await _groupMemberService.approvalParticipationRequest(request);
            }
            catch (Exception e)
            {
                _result.HandleException(e);
            }
            return Ok(_result);
        }
    }
}
