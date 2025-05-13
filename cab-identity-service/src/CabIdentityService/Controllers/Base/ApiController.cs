using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WCABNetwork.Cab.IdentityService.Controllers.Base
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class ApiController<T> : ControllerBase
    {
        protected readonly ILogger<T> _logger;

        public ApiController(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}