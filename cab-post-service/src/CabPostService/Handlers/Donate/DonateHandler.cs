using AutoMapper;
using CabPostService.Handlers.Base;

namespace CabPostService.Handlers.Donate
{
    public partial class DonateHandler :
        BaseHandler<DonateHandler>
    {
        public DonateHandler(
           IServiceProvider serviceProvider,
           ILogger<DonateHandler> logger,
           IHttpContextAccessor httpContextAccessor,
           IMapper mapper) :
            base(serviceProvider, logger, httpContextAccessor, mapper)
        {
        }
    }
}

