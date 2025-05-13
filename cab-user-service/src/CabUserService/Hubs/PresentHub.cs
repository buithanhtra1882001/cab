using CabUserService.Infrastructures.DbContexts;
using CabUserService.Models.Entities;
using LazyCache;
using MassTransit.Initializers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace CabUserService.Hubs
{
    public class PresentHub : Hub
    {
        private readonly IServiceProvider _serviceProvider;

        public PresentHub(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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

        public override async Task OnDisconnectedAsync(Exception exception)
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
    }
}
