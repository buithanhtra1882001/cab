
using CabPostService.Controllers.Base;
using CabPostService.Cqrs.Requests.Commands;
using CabPostService.Cqrs.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CabPostService.Controllers
{
    public class UsersController : ApiController<UsersController>
    {
        #region Properties

        private readonly IMediator _mediator;

        #endregion Properties

        #region Constructor

        public UsersController(ILogger<UsersController> logger, IMediator mediator) : base(logger)
        {
            _mediator = mediator;
        }

        #endregion Constructor

        #region Method

        /// <summary>
        /// update profile user
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("statistical-user"), DisableRequestSizeLimit]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> StatisticalUserAsync([FromQuery] StatisticalUserQuery command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Donate([FromBody] DonateReceiverCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("handle-donate")]
        public async Task<IActionResult> HandleDonate([FromBody] HandleDonateReceiverCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        #endregion
    }
}