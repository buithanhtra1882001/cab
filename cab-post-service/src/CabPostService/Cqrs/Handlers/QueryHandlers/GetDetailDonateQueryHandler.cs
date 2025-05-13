using MediatR;
using CabPostService.Cqrs.Requests.Queries;
using CabPostService.Models.Dtos;
using CabPostService.Services.Abstractions;

namespace CabPostService.Cqrs.Handlers.QueryHandlers
{
    public class GetDetailDonateQueryHandler : IRequestHandler<GetDetailDonateQuery, List<DonateDetailResponse>>
    {
        #region properties

        private readonly IDonateService _donateService;
        
        #endregion

        #region Constructor

        public GetDetailDonateQueryHandler(IDonateService donateService)
        {
            _donateService = donateService;
        }

        #endregion

        #region Methods

        public virtual Task<List<DonateDetailResponse>> Handle(GetDetailDonateQuery request, CancellationToken cancellationToken)
        {
            return _donateService.GetDetailDonateAsync(request, cancellationToken);
        }

        #endregion
    }
}

