using CabUserService.DomainCommands.Commands;
using CabUserService.Infrastructures.Exceptions;
using CabUserService.Infrastructures.Repositories.Interfaces;
using MediatR;

namespace CabUserService.DomainCommands.CommandHandlers
{
    public class UpdateUserDetailCommandHandler : INotificationHandler<UpdateUserDetailCommand>
    {
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly ILogger<UpdateUserDetailCommandHandler> _logger;

        public UpdateUserDetailCommandHandler(IUserDetailRepository UserDetailRepository, ILogger<UpdateUserDetailCommandHandler> logger)
        {
            _userDetailRepository = UserDetailRepository;
            _logger = logger;
        }

        public async Task Handle(UpdateUserDetailCommand notification, CancellationToken cancellationToken)
        {
            try
            {
                notification.UserDetail.UpdatedAt = DateTime.Now;

                await _userDetailRepository.UpdateAsync(notification.UserDetail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on UpdateUserDetailCommandHandler.Handle");
                throw new CommandHandlerException(ex.Message);
            }
        }
    }
}
