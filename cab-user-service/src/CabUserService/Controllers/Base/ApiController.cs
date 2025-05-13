using Microsoft.AspNetCore.Mvc;

namespace CabUserService.Controllers.Base
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class ApiController<T> : ControllerBase
    {
        protected readonly ILogger<T> _logger;
        protected string BearerToken
        {
            get
            {
                return Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            }
        }

        public ApiController(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}