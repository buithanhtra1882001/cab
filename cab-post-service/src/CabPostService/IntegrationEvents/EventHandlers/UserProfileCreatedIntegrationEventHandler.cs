using AutoMapper;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CabPostService.Infrastructures.DbContexts;
using CabPostService.IntegrationEvents.Events;
using CabPostService.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CabPostService.IntegrationEvents.EventHandlers
{
    public class UserProfileCreatedIntegrationEventHandler :
        IIntegrationEventHandler<UserProfileCreatedIntegrationEvent>
    {
        private readonly ILogger<UserProfileCreatedIntegrationEventHandler> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public UserProfileCreatedIntegrationEventHandler(
            ILogger<UserProfileCreatedIntegrationEventHandler> logger,
            IServiceProvider serviceProvider,
            IMapper mapper)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider;
            _mapper = mapper;
        }
        public async Task Handle(UserProfileCreatedIntegrationEvent @event)
        {
            try
            {
                _logger.LogInformation($"Consume UserProfileCreatedIntegrationEvent with eventId {@event.Id} at {@event.CreationDate.ToString("dd-MM-yyyy HH:mm:ss")}");

                // update insert from postesdb 08/01/2024
                using var scope = _serviceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();

                var userMiniEntity = await db.Users.FirstOrDefaultAsync(item => item.Id == @event.UserId);
                if (userMiniEntity != null)
                {
                    userMiniEntity.Avatar = @event.Avatar;
                    userMiniEntity.Fullname = @event.Fullname;
                    userMiniEntity.Username = @event.Username;
                    db.Users.Update(userMiniEntity);
                }
                else
                {
                    var userMini = new PostMiniUser
                    {
                        Id = @event.UserId,
                        Avatar = @event.Avatar,
                        Fullname = @event.Fullname,
                        Username = @event.Username
                    };
                    db.Users.Add(userMini);
                }
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error at UserProfileCreatedIntegrationEvent: {ex.Message}");
            }
        }
    }
}
