using CabUserService.Models.Entities;
using MediatR;

namespace CabUserService.DomainCommands.Commands
{
    public class UpdateUserDetailCommand : INotification
    {
        public UserDetail UserDetail { get; set; }
    }
}
