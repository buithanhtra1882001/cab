using CabUserService.Controllers.Base;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;
using CabUserService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Controllers
{
    public class ProfileAdminController : ApiController<ProfileAdminController>
    {
        public readonly IProfileService _profileService;
        public ProfileAdminController(ILogger<ProfileAdminController> logger,
            IProfileService userService)
            : base(logger)
        {
            _profileService = userService;
        }

        /// <summary>
        /// Admin search users.
        /// </summary>
        /// <response code="200">Search users successful</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpPost("Search")]
        [ProducesResponseType(typeof(PagingResponse<User>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Search([FromBody] GetAllUserRequest request)
        {
            var profile = await _profileService.AdminGetListAsync(request);
            return Ok(profile);
        }

        /// <summary>
        /// Admin get an user public information.
        /// </summary>
        /// <response code="200">Get an user public information successful</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpGet("{cabUserId}")]
        [ProducesResponseType(typeof(PublicUserInformationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProfile([Required] string userRole, [Required] Guid cabUserId)
        {
            var httpMessageResponse = new HttpMessageResponse();
            var profile = await _profileService.AdminGetProfileAsync(cabUserId, userRole);
            if (profile is null)
            {
                httpMessageResponse.Message = "Profile not found";
                return NotFound(httpMessageResponse);
            }
            return Ok(profile);
        }

        /// <summary>
        /// Admin delete an user.
        /// </summary>
        /// <response code="200">Admin delete an user successful</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpDelete("{cabUserId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser([Required] Guid auid, [Required] Guid cabUserId)
        {
            var httpMessageResponse = new HttpMessageResponse();
            var deletedUserId = await _profileService.AdminDeleteUserAsync(auid, cabUserId);
            if (deletedUserId is null)
            {
                httpMessageResponse.Message = "Delete user is failed.";
                return NotFound(httpMessageResponse);
            }
            return Ok(deletedUserId);
        }
    }
}