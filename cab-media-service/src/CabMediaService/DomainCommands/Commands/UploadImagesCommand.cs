using System.Collections.Generic;
using CabMediaService.DomainCommands.Commands.Base;
using CabMediaService.Models.Entities;

namespace CabMediaService.DomainCommands.Commands
{
    public class UploadImagesCommand : BaseCommand
    {
        public ICollection<MediaImage> MediaImages { get; set; }
    }
}