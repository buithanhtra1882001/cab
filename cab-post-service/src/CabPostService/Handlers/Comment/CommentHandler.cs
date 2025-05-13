using AutoMapper;
using CabPostService.Handlers.Base;

namespace CabPostService.Handlers.Comment
{
    public partial class CommentHandler : BaseHandler<CommentHandler>
    {
        public CommentHandler(
           IServiceProvider serviceProvider,
           ILogger<CommentHandler> logger,
           IHttpContextAccessor httpContextAccessor,
           IMapper mapper) : base(serviceProvider, logger, httpContextAccessor, mapper)
        {
        }
    }
}
