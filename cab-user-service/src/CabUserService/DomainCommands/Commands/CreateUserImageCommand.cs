using CabUserService.Models.Entities;
using MediatR;

namespace CabUserService.DomainCommands.Commands
{
    public class CreateUserImageCommand : INotification
    {
        public List<UserImage> UserImages { get; set; }
    }
}
