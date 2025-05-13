using MediatR;
using CabUserService.Cqrs.Requests.Queries;
using CabUserService.Models.Dtos;
using CabUserService.Services.Interfaces;

namespace TecsPjService.Apis.Cqrs.QueryHandlers.Address
{
    public class GetStatisticalUserQueryQueryHandler : IRequestHandler<StatisticalUserQuery, StatisticalUserDto>
    {
        #region properties

        private readonly IUserService _userService;
        
        #endregion

        #region Constructor

        public GetStatisticalUserQueryQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        #region Methods

        public virtual Task<StatisticalUserDto> Handle(StatisticalUserQuery request, CancellationToken cancellationToken)
        {
            return _userService.GetStatisticalUserAsync(request, cancellationToken);
        }

        #endregion
    }
}
