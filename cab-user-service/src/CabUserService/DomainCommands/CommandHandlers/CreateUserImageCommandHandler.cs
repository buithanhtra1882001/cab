using CabUserService.DomainCommands.Commands;
using CabUserService.Infrastructures.Exceptions;
using CabUserService.Infrastructures.Repositories.Interfaces;
using MediatR;

namespace CabUserService.DomainCommands.CommandHandlers
{
    public class CreateUserImageCommandHandler : INotificationHandler<CreateUserImageCommand>
    {
        private readonly IUserImageRepository _userImageRepository;
        private readonly ILogger<CreateUserImageCommandHandler> _logger;

        public CreateUserImageCommandHandler(IUserImageRepository userImageRepository, ILogger<CreateUserImageCommandHandler> logger)
        {
            _userImageRepository = userImageRepository;
            _logger = logger;
        }

        public async Task Handle(CreateUserImageCommand notification, CancellationToken cancellationToken)
        {
            try
            {
                await _userImageRepository.CreateAsync(notification.UserImages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on CreateUserImageCommand.Handle");
                throw new CommandHandlerException(ex.Message);
            }
        }
    }
}
