using CabUserService.DomainCommands.Commands;
using CabUserService.Infrastructures.Exceptions;
using CabUserService.Infrastructures.Repositories.Interfaces;
using MediatR;

namespace CabUserService.DomainCommands.CommandHandlers
{
    public class DeleteUserCommandHandler : INotificationHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<DeleteUserCommandHandler> _logger;

        public DeleteUserCommandHandler(IUserRepository userRepository, ILogger<DeleteUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }
        public async Task Handle(DeleteUserCommand notification, CancellationToken cancellationToken)
        {
            try
            {
                await _userRepository.HardDeleteAsync(notification.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on DeleteUserCommandHandler.Handle");
                throw new CommandHandlerException(ex.Message);
            }
        }
    }
}
