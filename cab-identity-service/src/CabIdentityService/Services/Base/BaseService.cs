using Microsoft.Extensions.Logging;

namespace WCABNetwork.Cab.IdentityService.Services.Base
{
    public class BaseService<T>
    {
        protected readonly ILogger<T> _logger;

        public BaseService(ILogger<T> logger)
        {
            _logger = logger;
        }
    }
}