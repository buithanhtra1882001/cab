using MediatR;
using CabPostService.Cqrs.Requests.Queries;
using CabPostService.Models.Dtos;
using CabPostService.Services.Abstractions;

namespace CabPostService.Cqrs.Handlers.QueryHandlers
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

