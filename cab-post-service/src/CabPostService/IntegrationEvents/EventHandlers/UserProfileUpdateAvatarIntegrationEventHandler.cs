using AutoMapper;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CabPostService.Constants;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Helpers;
using CabPostService.IntegrationEvents.Events;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using LazyCache;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.IntegrationEvents.EventHandlers
{
    public class UserProfileUpdateAvatarIntegrationEventHandler : IIntegrationEventHandler<UserProfileUpdateAvatarIntegrationEvent>
    {
        private readonly ILogger<UserProfileUpdateAvatarIntegrationEventHandler> _logger;
        private readonly IAppCache _cache;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public UserProfileUpdateAvatarIntegrationEventHandler(ILogger<UserProfileUpdateAvatarIntegrationEventHandler> logger, IServiceProvider serviceProvider, IMapper mapper, IAppCache cache)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task Handle(UserProfileUpdateAvatarIntegrationEvent @event)
        {
            try
            {
                _logger.LogInformation($"Consume UserProfileUpdateAvatarIntegrationEvent with eventId {@event.Id} at {@event.CreationDate.ToString("dd-MM-yyyy HH:mm:ss")}");

                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();

                var userMini = await db.Users.FirstOrDefaultAsync(x => x.Id == @event.UserId);
                if (userMini != null)
                {
                    userMini.Avatar = @event.Avatar;
                    db.Users.Update(userMini);
                    await db.SaveChangesAsync();


                    UpdatePostCache(@event);
                    UpdateCommentCache(@event);
                }
                else
                {
                    _logger.LogError($"Not found user with Id = {@event.UserId}");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at UserProfileCreatedIntegrationEvent: {ex.Message}");
            }
        }
        private void UpdatePostCache(UserProfileUpdateAvatarIntegrationEvent @event)
        {
            var posts = _cache.Get<List<UserPostResponse>>(CacheKeyConstant.POSTS);
            if (posts is null)
                return;

            var updatedPosts = posts.Where(item => item.UserId == @event.UserId).ToList();
            foreach (var item in updatedPosts)
            {
                item.UserAvatar = @event.Avatar;
            }
        }

        private void UpdateCommentCache(UserProfileUpdateAvatarIntegrationEvent @event)
        {
            var commentCaches = CacheHelper.GetAllCacheEntries(_cache.CacheProvider)
                .Where(item => item.Key.ToString()
                .Contains(CacheKeyConstant.POST_COMMENTS));

            var commentValues = commentCaches
                .SelectMany(item => (item.Value as AsyncLazy<List<UserCommentResponse>>).Value.Result)
                .Where(item => item.UserId == @event.UserId)
                .ToList();

            foreach (var item in commentValues)
            {
                item.Avatar = @event.Avatar;
            }
        }
    }
}
