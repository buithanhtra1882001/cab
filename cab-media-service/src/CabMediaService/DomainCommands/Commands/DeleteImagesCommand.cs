using System;
using System.Collections.Generic;
using CabMediaService.DomainCommands.Commands.Base;

namespace CabMediaService.DomainCommands.Commands
{
    public class DeleteImagesCommand : BaseCommand
    {
        public IEnumerable<Guid> Ids { get; set; }
    }
}