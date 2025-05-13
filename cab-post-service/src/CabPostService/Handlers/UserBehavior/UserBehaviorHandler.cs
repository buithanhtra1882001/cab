using AutoMapper;
using CabPostService.Handlers.Base;

namespace CabPostService.Handlers.UserBehavior
{
    public partial class UserBehaviorHandler : BaseHandler<UserBehaviorHandler>
    {
        public UserBehaviorHandler(
            IServiceProvider serviceProvider,
            ILogger<UserBehaviorHandler> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper) :
            base(serviceProvider, logger, httpContextAccessor, mapper)
        {
        }
    }
}
