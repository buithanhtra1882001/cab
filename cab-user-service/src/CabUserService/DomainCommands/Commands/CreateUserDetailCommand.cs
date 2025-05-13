using CabUserService.Models.Entities;
using MediatR;

namespace CabUserService.DomainCommands.Commands
{
    public class CreateUserDetailCommand : INotification
    {
        public UserDetail UserDetail { get; set; }
        public int SequenceId { get; set; }
    }
}
