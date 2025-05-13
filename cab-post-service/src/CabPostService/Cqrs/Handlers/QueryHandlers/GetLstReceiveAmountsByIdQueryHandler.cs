using MediatR;
using CabPostService.Cqrs.Requests.Queries;
using CabPostService.Models.Dtos;
using CabPostService.Services.Abstractions;

namespace CabPostService.Cqrs.Handlers.QueryHandlers
{
    public class GetLstReceiveAmountsByIdQueryHandler : IRequestHandler<GetLstReceiveAmountsByIdQuery, List<GetLstReceiveAmountsByIdResponse>>
    {
        #region properties

        private readonly IDonateService _donateService;
        
        #endregion

        #region Constructor

        public GetLstReceiveAmountsByIdQueryHandler(IDonateService donateService)
        {
            _donateService = donateService;
        }

        #endregion

        #region Methods

        public virtual Task<List<GetLstReceiveAmountsByIdResponse>> Handle(GetLstReceiveAmountsByIdQuery request, CancellationToken cancellationToken)
        {
            return _donateService.GetLstReceiveAmountsByIdAsync(request, cancellationToken);
        }

        #endregion
    }
}

