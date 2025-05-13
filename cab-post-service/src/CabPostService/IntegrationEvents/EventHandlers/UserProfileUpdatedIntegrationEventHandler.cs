using AutoMapper;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CabPostService.Constants;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.Infrastructures.Helpers;
using CabPostService.Infrastructures.Repositories.Interfaces;
using CabPostService.IntegrationEvents.Events;
using CabPostService.Models.Dtos;
using CabPostService.Models.Entities;
using LazyCache;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.IntegrationEvents.EventHandlers
{
    public class UserProfileUpdatedIntegrationEventHandler :
        IIntegrationEventHandler<UserProfileUpdatedIntegrationEvent>
    {
        private readonly ILogger<UserProfileUpdatedIntegrationEventHandler> _logger;
        private readonly IAppCache _cache;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public UserProfileUpdatedIntegrationEventHandler(
            ILogger<UserProfileUpdatedIntegrationEventHandler> logger,
            IAppCache cache,
            IServiceProvider serviceProvider,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache;
            _serviceProvider = serviceProvider;
            _mapper = mapper;
        }
        public async Task Handle(UserProfileUpdatedIntegrationEvent @event)
        {
            try
            {
                _logger.LogInformation($"Consume UserProfileUpdatedIntegrationEvent with eventId {@event.Id} at {@event.CreationDate.ToString("dd-MM-yyyy HH:mm:ss")}");

                // update from postesdb 08/01/2024
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();

                var userMini = await db.Users.FirstOrDefaultAsync(item => item.Id == @event.UserId);
                if (userMini is null) return;

                userMini.Fullname = @event.Fullname;
                userMini.Username = @event.Username;
                userMini.Avatar = @event.Avatar;
                userMini.UpdatedAt = DateTime.UtcNow;

                db.Users.Update(userMini);
                await db.SaveChangesAsync();

                UpdatePostCache(@event);
                UpdateCommentCache(@event);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at UserProfileUpdatedIntegrationEvent: {ex.Message}");
            }
        }

        private void UpdatePostCache(UserProfileUpdatedIntegrationEvent @event)
        {
            var posts = _cache.Get<List<UserPostResponse>>(CacheKeyConstant.POSTS);
            if (posts is null)
                return;

            var updatedPosts = posts.Where(item => item.UserId == @event.UserId).ToList();
            foreach (var item in updatedPosts)
            {
                item.UserFullName = @event.Fullname;
                item.UserAvatar = @event.Avatar;
            }
        }

        private void UpdateCommentCache(UserProfileUpdatedIntegrationEvent @event)
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
                item.UserFullName = @event.Fullname;
                item.Avatar = @event.Avatar;
            }
        }
    }
}
