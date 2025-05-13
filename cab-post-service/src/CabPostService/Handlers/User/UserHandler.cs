using AutoMapper;
using CabPostService.Handlers.Base;

namespace CabPostService.Handlers.User
{
    public partial class UserHandler : BaseHandler<UserHandler>
    {
        public UserHandler(
            IServiceProvider serviceProvider,
            ILogger<UserHandler> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper) :
            base(serviceProvider, logger, httpContextAccessor, mapper)
        {
        }
    }
}
