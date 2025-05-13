using CAB.BuildingBlocks.EventBus.Abstractions;
using CabMediaService.DomainCommands.CommandHandlers.Base;
using CabMediaService.DomainCommands.Commands;
using CabMediaService.Infrastructures.Repositories.Interfaces;
using CabMediaService.IntegrationEvents.Events;
using CabMediaService.Models.Entities;

namespace CabMediaService.DomainCommands.CommandHandlers
{
    public class DeleteImagesCommandHandler : BaseCommandHandler<DeleteImagesCommandHandler, DeleteImagesCommand>
    {
        private readonly IMediaImageRepository _imageRepository;

        public DeleteImagesCommandHandler(IEventBus eventBus
            , ILogger<DeleteImagesCommandHandler> logger
            , IMediaImageRepository imageRepository) : base(logger, eventBus)
        {
            _imageRepository = imageRepository;
        }

        protected override async Task<IEnumerable<EventCreatedIntegrationEvent>> Handle(DeleteImagesCommand notification)
        {
            foreach (var item in notification.Ids)
                await _imageRepository.DeleteAsync(x => x.Id == item);
            
            return notification.Ids
                .Select(x => new EventCreatedIntegrationEvent(x.ToString()
                    , nameof(MediaImage)
                    , DELETED
                    , string.Empty
                    , notification.UserId.ToString()));
        }
    }
}