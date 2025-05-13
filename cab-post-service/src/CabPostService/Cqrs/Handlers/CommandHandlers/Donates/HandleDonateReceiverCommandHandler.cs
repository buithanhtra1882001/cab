using CabPostService.Cqrs.Requests.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Services.Abstractions;
using MediatR;

namespace TecsPjService.Apis.Cqrs.CommandHandlers.Employee
{
    public class HandleDonateReceiverCommandHandler : IRequestHandler<HandleDonateReceiverCommand, bool>
    {
        #region Properties

        private readonly IDonateService _donateService;

        #endregion

        #region Constructor

        public HandleDonateReceiverCommandHandler(IDonateService donateService)
        {
            _donateService = donateService;
        }

        #endregion

        #region Method
        
        public virtual Task<bool> Handle(HandleDonateReceiverCommand command, CancellationToken cancellationToken)
        {
            return _donateService.HandleDonateReceiverAsync(command, cancellationToken);
        }
        
        #endregion
    }
}
