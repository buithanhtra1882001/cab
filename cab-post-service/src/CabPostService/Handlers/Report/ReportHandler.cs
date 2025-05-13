using AutoMapper;
using CabPostService.Handlers.Base;

namespace CabPostService.Handlers.Report
{
    public partial class ReportHandler :
        BaseHandler<ReportHandler>
    {
        public ReportHandler(
             IServiceProvider serviceProvider,
             ILogger<ReportHandler> logger,
             IHttpContextAccessor httpContextAccessor,
             IMapper mapper) : base(serviceProvider, logger, httpContextAccessor, mapper)
        {
        }
    }
}
