using CabPostService.Constants;
using CabPostService.DomainCommands.Commands;
using CabPostService.Infrastructures.Repositories;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.Models.Entities;
using MediatR;

namespace CabPostService.DomainCommands.CommandHandlers
{
    public class UpdateHashtagCommandHandler : INotificationHandler<UpdateHashtagCommand>
    {
        private readonly ILogger<UpdateHashtagCommandHandler> _logger;
        private readonly IPostRepository _postRepository;
        private readonly IPostHashtagRepository _postHashtagRepository;
        public UpdateHashtagCommandHandler(
            IPostRepository postRepository, 
            IPostHashtagRepository postHashtagRepository, 
            ILogger<UpdateHashtagCommandHandler> logger)
        {
            _postRepository = postRepository;
            _postHashtagRepository = postHashtagRepository;
            _logger = logger;
        }

        public async Task Handle(UpdateHashtagCommand notification, CancellationToken cancellationToken)
        {
            try
            {
                string hashtags = notification.Hashtags;
                if (!string.IsNullOrEmpty(hashtags))
                {
                    var lstHashtag = hashtags.Split(",");
                    foreach (var item in lstHashtag)
                    {
                        var nameHashtag = item.Trim();
                        var lstHashtagByName = await _postHashtagRepository.GetByName(nameHashtag);
                        if (lstHashtagByName.Count() > 0)
                        {
                            foreach (var hashTag in lstHashtagByName)
                            {
                                hashTag.Point += (HashtagConstants.POINT_ACTION);
                                hashTag.UpdatedAt = DateTime.UtcNow;
                                await _postHashtagRepository.UpdateAsync(hashTag);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on UpdateHashtagCommand.Handle");
                throw ex;
            }
        }
    }
}
