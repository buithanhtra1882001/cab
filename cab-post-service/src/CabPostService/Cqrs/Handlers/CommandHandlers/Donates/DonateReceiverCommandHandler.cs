using CabPostService.Cqrs.Requests.Commands;
using CabPostService.Models.Dtos;
using CabPostService.Services.Abstractions;
using MediatR;

namespace TecsPjService.Apis.Cqrs.CommandHandlers.Employee
{
    public class DonateReceiverCommandHandler : IRequestHandler<DonateReceiverCommand, ResponseDto>
    {
        #region Properties

        private readonly IDonateService _donateService;

        #endregion

        #region Constructor

        public DonateReceiverCommandHandler(IDonateService donateService)
        {
            _donateService = donateService;
        }

        #endregion

        #region Method
        
        public virtual Task<ResponseDto> Handle(DonateReceiverCommand command, CancellationToken cancellationToken)
        {
            return _donateService.DonateReceiverAsync(command, cancellationToken);
        }
        
        #endregion
    }
}
