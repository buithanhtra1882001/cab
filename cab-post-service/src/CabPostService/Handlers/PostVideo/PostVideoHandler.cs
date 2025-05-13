using AutoMapper;
using CabPostService.Handlers.Base;

namespace CabPostService.Handlers.PostVideo
{
    public partial class PostVideoHandler : BaseHandler<PostVideoHandler>
    {
        public PostVideoHandler(
            IServiceProvider serviceProvider,
            ILogger<PostVideoHandler> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper) : base(serviceProvider, logger, httpContextAccessor, mapper)
        {
        }
    }
}