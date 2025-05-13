using AutoMapper;
using CabPostService.Handlers.Base;

namespace CabPostService.Handlers.PostCategory
{
    public partial class PostCategoryHandler :
        BaseHandler<PostCategoryHandler>
    {
        public PostCategoryHandler(
           IServiceProvider serviceProvider,
           ILogger<PostCategoryHandler> logger,
           IHttpContextAccessor httpContextAccessor,
           IMapper mapper) : base(serviceProvider, logger, httpContextAccessor, mapper)
        {
        }
    }
}

