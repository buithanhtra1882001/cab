using AutoMapper;
using CabGroupService.Handlers.Base;
using CabGroupService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CabGroupService.Handlers.Notification
{
    public partial class NotificationHandler : BaseHandler<NotificationHandler>
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IConfiguration _configuration;
        public NotificationHandler(
            IServiceProvider serviceProvider,
            ILogger<NotificationHandler> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper,
            IHubContext<NotificationHub> hubContext,
            IConfiguration configuration)
            : base(serviceProvider, logger, httpContextAccessor, mapper)
        {
            _hubContext = hubContext;
            _configuration = configuration;
        }
    }
}
