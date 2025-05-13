using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Commands;
using CabPostService.Models.Entities;

namespace CabPostService.Handlers.Report
{
    public partial class ReportHandler :
        ICommandHandler<ReportCommand, Guid>
    {
        public async Task<Guid> Handle(
            ReportCommand request,
            CancellationToken cancellationToken)
        {
            var report = _mapper.Map<PostReport>(request);
            report.Id = Guid.NewGuid();

            var postReportRepository = _seviceProvider.GetRequiredService<IPostReportRepository>();
            await postReportRepository.CreateAsync(report);

            return report.Id;
        }
    }
}
