using Microsoft.AspNetCore.Identity;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WCABNetwork.Cab.IdentityService.IntegrationEvents.Events;
using WCABNetwork.Cab.IdentityService.Models.Entities;

namespace WCABNetwork.Cab.IdentityService.IntegrationEvents.EventHandlers
{
    public class UserDeletedIntegrationEventHandler : IIntegrationEventHandler<UserDeletedIntegrationEvent>
    {
        private readonly ILogger<UserDeletedIntegrationEventHandler> _logger;
        private readonly UserManager<Account> _userManager;

        public UserDeletedIntegrationEventHandler(
            ILogger<UserDeletedIntegrationEventHandler> logger, UserManager<Account> userManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userManager = userManager;
        }
        public async Task Handle(UserDeletedIntegrationEvent @event)
        {
            try
            {
                _logger.LogInformation($"Consume eventId {@event.Id} at {@event.CreationDate.ToString("dd-MM-yyyy HH:mm:ss")}");
                var account = await _userManager.FindByEmailAsync(@event.Email);
                if (account != null && !account.IsSoftDeleted)
                {
                    account.IsSoftDeleted = true;
                    await _userManager.UpdateAsync(account);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at UserBannedIntegrationEventHandler: {ex.Message}");
            }
            
        }
    }
}
