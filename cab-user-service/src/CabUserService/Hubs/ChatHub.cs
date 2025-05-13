using AutoMapper;
using CAB.BuildingBlocks.EventBus.Abstractions;
using CabNotificationService.Constants;
using CabUserService.Infrastructures.DbContexts;
using CabUserService.Infrastructures.Repositories.Interfaces;
using CabUserService.IntegrationEvents.Events;
using CabUserService.Models.Dtos;
using CabUserService.Models.Entities;
using MassTransit.Initializers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace CabUserService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;
        public ChatHub(IServiceProvider serviceProvider, IMapper mapper)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
                var token = Context.GetHttpContext().Request.Query["access_token"].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    return;
                }
                var userId = GetCurrentUserIdInTokenAsync(token);
                if (userId == Guid.Empty)
                {
                    return;
                }
                var userIdEntity = await db.Users
                                  .FirstOrDefaultAsync(x => x.Id == userId).Select(x => x.Id);
                if (userIdEntity == Guid.Empty)
                {
                    var errorMessage = $"Not found User with userId = {userId}";
                    throw new HubException(errorMessage);
                }


                var userConnectionEntity = await db.ChatUserConnections
                    .FirstOrDefaultAsync(x => x.UserId == userIdEntity && x.ConnectionId == Context.ConnectionId);

                if (userConnectionEntity is null)
                {
                    var userConnectionRequest = new ChatUserConnection
                    {
                        Id = Guid.NewGuid(),
                        UserId = userIdEntity,
                        ConnectionId = Context.ConnectionId,
                        IsOnline = true,
                    };
                    await db.ChatUserConnections.AddAsync(userConnectionRequest);
                }
                else
                {
                    userConnectionEntity.IsOnline = true;
                }
                await db.SaveChangesAsync();

            }
            catch (HubException ex)
            {
                throw new HubException(ex.Message);
            }
        }

        public async Task SendMessage(CreateMessageRequest request)
        {
            try
            {
                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
                var token = Context.GetHttpContext().Request.Query["access_token"].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    return;
                }
                var senderUserId = GetCurrentUserIdInTokenAsync(token);
                if (senderUserId == Guid.Empty)
                {
                    return;
                }
                request.SenderUserId = senderUserId;
                var chatMessageRepository = _serviceProvider.GetRequiredService<IChatMessageRepository>();
                var senderEntity = await db.Users.FirstOrDefaultAsync(x => x.Id == request.SenderUserId);
                var recipientEntity = await db.Users.FirstOrDefaultAsync(x => x.Id == request.RecipientUserId);

                if (senderEntity.Id == Guid.Empty || recipientEntity.Id == Guid.Empty)
                {
                    var errorMessage = $"Not found User with userId = {senderEntity.Id}-{request.SenderUserId} " +
                        $"|| userFriendId = {recipientEntity.Id}-{request.RecipientUserId}";
                    throw new HubException(errorMessage);
                }
                if (senderEntity.Id == request.RecipientUserId)
                {
                    var errorMessage = "You cannot send it to yourself";
                    throw new HubException(errorMessage);
                }

                var messageRequest = _mapper.Map<ChatMessage>(request);
                messageRequest.Id = Guid.NewGuid();
                var messageResponse = _mapper.Map<MessagesResponse>(messageRequest);

                messageResponse.SenderName = senderEntity.FullName;
                messageResponse.RecipientName = recipientEntity.FullName;

                var connectionIds = await db.ChatUserConnections
                    .Where(x => x.UserId == recipientEntity.Id)
                    .Select(x => x.ConnectionId).ToListAsync();
                if (connectionIds.Any())
                {
                    await Clients.Clients(connectionIds).SendAsync("NewMessage", messageResponse);
                }

                await Clients.Caller.SendAsync("NewMessage", messageResponse);

                await chatMessageRepository.CreateAsync(messageRequest);

                var eventBus = _serviceProvider.GetRequiredService<IEventBus>();
                var actorInfo = await GetSystemActorInForAsync();

                eventBus.Publish(new NotificationIntegrationEvent
                       (
                        userIds: new List<Guid> { recipientEntity.Id },
                        actor: actorInfo,
                        referenceId: recipientEntity.Id,
                        notificationType: NotificationConstant.Chat,
                        donateAmount: null,
                        referenceUrl: null,
                        type: NotificationConstant.System
                        ));

            }
            catch (HubException ex)
            {
                throw new HubException(ex.Message);
            }

        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var db = _serviceProvider.GetRequiredService<PostgresDbContext>();
                var token = Context.GetHttpContext().Request.Query["access_token"].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    return;
                }
                var userId = GetCurrentUserIdInTokenAsync(token);
                if (userId == Guid.Empty)
                {
                    return;
                }
                var userIdEntity = await db.Users
                                 .FirstOrDefaultAsync(x => x.Id == userId).Select(x => x.Id);
                if (userIdEntity == Guid.Empty)
                {
                    var errorMessage = $"Not found User with userId = {userId}";
                    throw new HubException(errorMessage);
                }
                var userConnectionEntity = await db.ChatUserConnections
                    .FirstOrDefaultAsync(x => x.UserId == userIdEntity && x.ConnectionId == Context.ConnectionId);
                if (userConnectionEntity is null)
                {
                    var errorMessage = $"Not found User with userId = {userId}";
                    throw new Exception(errorMessage);
                }
                /*nếu remove thì không get được user online offline, chạy job 00h xóa user có isOnline = false */
                //db.ChatUserConnections.Remove(userConnectionEntity);
                userConnectionEntity.IsOnline = false;
                userConnectionEntity.LastTime = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
            catch (HubException ex)
            {
                throw new HubException(ex.Message);
            }
        }

        private Guid GetCurrentUserIdInTokenAsync(string token)
        {
            var jwtToken = new JwtSecurityToken(token);
            var uuid = jwtToken.Claims.FirstOrDefault(x => x.Type.ToLowerInvariant() == "uuid").Value;
            return Guid.Parse(uuid);
        }
        private async Task<UserInfo> GetSystemActorInForAsync()
        {
            var db = _serviceProvider.GetRequiredService<PostgresDbContext>();

            return await db.Users.Include(x => x.UserDetail)
                     .FirstOrDefaultAsync(x => x.FullName == "Hệ thống")
                     .Select(x => new UserInfo
                     (
                         userId: x.Id,
                         fullName: x.FullName,
                         avatar: x.UserDetail.Avatar
                     ));
        }
    }
}
