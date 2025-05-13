using AutoMapper;
using CabPostService.Handlers.Base;
using CabPostService.Infrastructures.Repositories.Interfaces;
using LazyCache;

namespace CabPostService.Handlers.SharePost
{
    public partial class SharePostHandler : BaseHandler<SharePostHandler>
    {
        protected ISharePostRepository SharePostRepository;
        protected IPostRepository PostRepository;
        protected IAppCache AppCache;
        protected IUserRepository UserRepository;

        public SharePostHandler(
          IServiceProvider serviceProvider,
          ISharePostRepository sharePostRepository,
          IPostRepository postRepository,
          IUserRepository userRepository,
          IAppCache appCache,
          ILogger<SharePostHandler> logger,
          IHttpContextAccessor httpContextAccessor,
          IMapper mapper)
          : base(serviceProvider, logger, httpContextAccessor, mapper)
        {
            SharePostRepository = sharePostRepository;
            PostRepository = postRepository;
            UserRepository = userRepository;
            AppCache = appCache;
        }
    }
}
