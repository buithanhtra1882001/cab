using CabPostService.Controllers.Base;
using CabPostService.Cqrs.Requests.Commands;
using CabPostService.Cqrs.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CabPostService.Controllers
{
    public class DonatePostController : ApiController<DonatePostController>
    {
        #region Properties

        private readonly IMediator _mediator;

        #endregion Properties

        #region Constructor

        public DonatePostController(ILogger<DonatePostController> logger, IMediator mediator) : base(logger)
        {
            _mediator = mediator;
        }

        #endregion Constructor

        #region Method

        [HttpPost]
        public async Task<IActionResult> DonateAsync([FromBody] DonateReceiverCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("handle-donate")]
        public async Task<IActionResult> HandleDonateAsync([FromBody] HandleDonateReceiverCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpGet("statistical-donate")]
        public async Task<IActionResult> HandlesSatisticalDonateAsync([FromQuery] StatisticalDonateQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("detail-donate")]
        public async Task<IActionResult> GetDetailDonateAsync([FromQuery] GetDetailDonateQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("receive-amounts")]
        public async Task<IActionResult> GetLstReceiveAmountsByIdAsync([FromQuery] GetLstReceiveAmountsByIdQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        #endregion
    }
}