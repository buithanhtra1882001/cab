using CAB.BuildingBlocks.EventBus.Abstractions;
using CabMediaService.DomainCommands.CommandHandlers.Base;
using CabMediaService.DomainCommands.Commands;
using CabMediaService.Infrastructures.Repositories.Interfaces;
using CabMediaService.IntegrationEvents.Events;
using CabMediaService.Models.Entities;

namespace CabMediaService.DomainCommands.CommandHandlers
{
    public class UploadImagesCommandHandler : BaseCommandHandler<UploadImagesCommandHandler, UploadImagesCommand>
    {
        private readonly IMediaImageRepository _imageRepository;

        public UploadImagesCommandHandler(IEventBus eventBus
            , ILogger<UploadImagesCommandHandler> logger
            , IMediaImageRepository imageRepository) : base(logger, eventBus)
        {
            _imageRepository = imageRepository;
        }

        protected override async Task<IEnumerable<EventCreatedIntegrationEvent>> Handle(UploadImagesCommand notification)
        {
            await _imageRepository.CreateManyAsync(notification.MediaImages);
            return notification.MediaImages
                .Select(x => new EventCreatedIntegrationEvent(x.Id.ToString()
                    , nameof(MediaImage)
                    , CREATED
                    , x.ToJson()
                    , notification.UserId.ToString()));
        }
    }
}