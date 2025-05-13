using MediatR;
using WCABNetwork.Cab.IdentityService.Models.Entities;

namespace WCABNetwork.Cab.IdentityService.DomainCommands.Commands
{
    public class UserRefreshTokenCreatedCommand : INotification
    {
        public Account Account { get; set; }
    }
}