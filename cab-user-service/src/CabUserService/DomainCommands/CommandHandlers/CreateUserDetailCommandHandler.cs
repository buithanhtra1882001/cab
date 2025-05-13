using CabUserService.DomainCommands.Commands;
using CabUserService.Infrastructures.Exceptions;
using CabUserService.Infrastructures.Repositories.Interfaces;
using MediatR;

namespace CabUserService.DomainCommands.CommandHandlers
{
    public class CreateUserDetailCommandHandler : INotificationHandler<CreateUserDetailCommand>
    {
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly ILogger<CreateUserDetailCommandHandler> _logger;

        public CreateUserDetailCommandHandler(IUserDetailRepository UserDetailRepository, ILogger<CreateUserDetailCommandHandler> logger)
        {
            _userDetailRepository = UserDetailRepository;
            _logger = logger;
        }

        public async Task Handle(CreateUserDetailCommand notification, CancellationToken cancellationToken)
        {
            try
            {
                await _userDetailRepository.CreateAsync(notification.UserDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on CreateUserDetailCommandHandler.Handle");
                throw new CommandHandlerException(ex.Message);
            }
        }
    }
}
