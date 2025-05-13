using AutoMapper;
using CabNotificationService.Handlers.Base;
using CabNotificationService.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CabNotificationService.Handlers.Notification
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
