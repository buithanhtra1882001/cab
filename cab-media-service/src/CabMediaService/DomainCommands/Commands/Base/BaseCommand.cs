using System;
using MediatR;

namespace CabMediaService.DomainCommands.Commands.Base
{
    public abstract class BaseCommand : INotification
    {
        public Guid UserId { get; set; }
    }
}