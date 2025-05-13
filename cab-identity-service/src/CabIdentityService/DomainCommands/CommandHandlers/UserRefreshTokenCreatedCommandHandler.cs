using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.DomainCommands.Commands;
using WCABNetwork.Cab.IdentityService.Infrastructures.Repositories.Interfaces;

namespace WCABNetwork.Cab.IdentityService.DomainCommands.CommandHandlers
{
    public class UserRefreshTokenCreatedCommandHandler : INotificationHandler<UserRefreshTokenCreatedCommand>
    {
        private readonly IUserRefreshTokenRepository _userRefreshTokenRepository;
        private readonly ILogger<UserRefreshTokenCreatedCommandHandler> _logger;

        public UserRefreshTokenCreatedCommandHandler(ILogger<UserRefreshTokenCreatedCommandHandler> logger,
            IUserRefreshTokenRepository userRefreshTokenRepository)
        {
            _userRefreshTokenRepository = userRefreshTokenRepository;
            _logger = logger;
        }

        public async Task Handle(UserRefreshTokenCreatedCommand notification, CancellationToken cancellationToken)
        {
            try
            {
                await _userRefreshTokenRepository.CleanUpExpiredTokensAsync(notification.Account.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on UserRefreshTokenCreatedCommandHandler.Handle");
            }
        }
    }
}