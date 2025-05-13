using AutoMapper;

namespace CabNotificationService.Handlers.Base
{
    public abstract class BaseHandler<T>
    {
        protected IServiceProvider _seviceProvider;
        protected ILogger<T> _logger;
        protected IHttpContextAccessor _httpContextAccessor;
        protected IMapper _mapper;

        protected BaseHandler(
            IServiceProvider serviceProvider,
            ILogger<T> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _seviceProvider = serviceProvider;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        protected string BearerToken
        {
            get
            {
                return _httpContextAccessor
                    .HttpContext
                    .Request
                    .Headers[Microsoft.Net.Http.Headers.HeaderNames.Authorization]
                    .ToString()
                    .Replace("Bearer ", "");
            }
        }
    }
}