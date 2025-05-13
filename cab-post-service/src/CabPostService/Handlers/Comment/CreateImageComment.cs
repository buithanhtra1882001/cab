using CAB.BuildingBlocks.EventBus.Abstractions;
using CabPostService.Constants;
using CabPostService.Handlers.Interfaces;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Exceptions;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.IntegrationEvents.Events;
using CabPostService.Models.Commands;
using CabPostService.Models.Entities;

namespace CabPostService.Handlers.Comment
{
    public partial class CommentHandler : ICommandHandler<CreateImageCommentCommand, Guid>
    {
        public async Task<Guid> Handle(CreateImageCommentCommand request, CancellationToken cancellationToken)
        {

            var db = _seviceProvider.GetRequiredService<PostgresDbContext>();
            var imageRepository = _seviceProvider.GetRequiredService<IPostImageRepository>();

            var imageEntity = await imageRepository.GetPostImageByIdAsync
                (request.ImageId);
            if (imageEntity is null)
            {
                _logger.LogWarning($"Not found image with imageId = {request.ImageId}");
                throw new EntityNotFoundException("The image is not found");
            }

            var postEntity = await db.Posts.FindAsync(imageEntity.PostId)
                ?? throw new EntityNotFoundException("The post is not found");

            var userEntity = await db.Users.FindAsync(request.UserId)
                ?? throw new EntityNotFoundException("The user is not found");

            var imageComment = _mapper.Map<ImageComment>(request);
            imageComment.Id = Guid.NewGuid();
            imageComment.Status = PostConstant.ACTIVE;

            var imageCommentRepository = _seviceProvider.GetRequiredService<IImageCommentRepository>();

            await imageCommentRepository.CreateAsync(imageComment);

            var actorInfo = new UserInfo(
                 userId: userEntity.Id,
                 fullName: userEntity.Fullname,
                 avatar: userEntity.Avatar
               );

            var eventBus = _seviceProvider.GetRequiredService<IEventBus>();
            eventBus.Publish(new NotificationIntegrationEvent
                (new List<Guid> { postEntity.UserId }, actorInfo, Guid.Parse(postEntity.Id), NotificationConstants.Comment));

            return imageComment.Id;
        }
    }
}
