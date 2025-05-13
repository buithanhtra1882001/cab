using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

namespace CabGroupService.Hubs
{
    public class NotificationHub : Hub
    {
        public readonly static ConnectionMapping<Guid> NotificationConnections = new ConnectionMapping<Guid>();
        public override async Task OnConnectedAsync()
        {
            try
            {
                var accessToken = Context.GetHttpContext()?.Request.Query["access_token"].ToString();
                if (string.IsNullOrEmpty(accessToken))
                    return;

                var currentUserId = GetCurrentUserIdInTokenAsync(accessToken);
                if (currentUserId == Guid.Empty)
                    return;

                RegisterConnection(currentUserId, Context.ConnectionId);
            }
            catch (HubException ex)
            {
                throw new HubException($"An error occurred while connecting.- {ex.Message}");
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            NotificationConnections.Remove(connectionId);

            return base.OnDisconnectedAsync(exception);
        }

        private Guid GetCurrentUserIdInTokenAsync(string token)
        {
            try
            {
                var jwtHandler = new JwtSecurityTokenHandler();
                var jwtToken = jwtHandler.ReadJwtToken(token);

                var uuid = jwtToken.Claims.FirstOrDefault(x => x.Type.ToLowerInvariant() == "uuid")?.Value;
                return Guid.Parse(uuid);
            }
            catch { }
            return Guid.Empty;
        }
        private void RegisterConnection(Guid currentUserId, string connectionId)
            => NotificationConnections.Add(currentUserId, connectionId);
    }
}
