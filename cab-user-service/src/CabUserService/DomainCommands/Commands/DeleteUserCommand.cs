using MediatR;

namespace CabUserService.DomainCommands.Commands
{
    public class DeleteUserCommand : INotification
    {
        public Guid Id { get; set; }
    }
}
