using CabUserService.DomainCommands.Commands;
using CabUserService.Infrastructures.Exceptions;
using CabUserService.Infrastructures.Repositories.Interfaces;
using MediatR;

namespace CabUserService.DomainCommands.CommandHandlers
{
    public class UpdateUserCommandHandler : INotificationHandler<UpdateUserDetailCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UpdateUserCommandHandler> _logger;
        public UpdateUserCommandHandler(IUserRepository userRepository, ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(UpdateUserDetailCommand notification, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(notification.UserDetail.UserId);
                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on UpdateUserCommandHandler.Handle");
                throw new CommandHandlerException(ex.Message);
            }
        }
    }
}
