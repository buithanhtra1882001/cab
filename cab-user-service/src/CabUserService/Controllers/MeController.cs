using CabUserService.Controllers.Base;
using CabUserService.Models.Dtos;
using CabUserService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Controllers
{
    public class MeController : ApiController<MeController>
    {
        public readonly IProfileService _profileService;

        public MeController(ILogger<MeController> logger,
            IProfileService userService)
            : base(logger)
        {
            _profileService = userService;
        }

        /// <summary>
        /// Get current user informations.
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet]
        [ProducesResponseType(typeof(FullUserInformationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCurrentUser([Required] Guid auid)
        {
            var response = new Response();
            var profile = await _profileService.UserGetProfileAsync(auid, null);

            if (profile is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "User not found";
            }
            else
            {
                response.StatusCode = StatusCodes.Status200OK;
                response.Data = profile;
            }

            return Ok(response);
        }
    }
}