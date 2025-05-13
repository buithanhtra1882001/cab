using MediatR;
using CabPostService.Cqrs.Requests.Queries;
using CabPostService.Models.Dtos;
using CabPostService.Services.Abstractions;

namespace CabPostService.Cqrs.Handlers.QueryHandlers
{
    public class StatisticalDonateQueryHandler : IRequestHandler<StatisticalDonateQuery, StatisticalDonateDto>
    {
        #region properties

        private readonly IDonateService _donateService;
        
        #endregion

        #region Constructor

        public StatisticalDonateQueryHandler(IDonateService donateService)
        {
            _donateService = donateService;
        }

        #endregion

        #region Methods

        public virtual Task<StatisticalDonateDto> Handle(StatisticalDonateQuery request, CancellationToken cancellationToken)
        {
            return _donateService.HandlesSatisticalDonateAsync(request, cancellationToken);
        }

        #endregion
    }
}

