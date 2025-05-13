using CabUserService.Models.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace CabUserService.Hubs
{
    /// <summary>
    /// SignalR hub for broadcasting donation notification to client streaming tool browser
    /// </summary>
    public class LiveDonationNotificationHub : Hub
    {
        public readonly static ConnectionMapping<string> LiveDonationConnections = new ConnectionMapping<string>();

        public async Task SendDonateNotification(DonateReceiverResponse message)
        {
            await Clients.Caller.SendAsync("ReceiveDonateNotification", message);
        }

        /// <summary>
        /// Register new connection on new connection established
        /// </summary>
        public override Task OnConnectedAsync()
        {
            var notiRecieverId = Context.GetHttpContext().Request.Query["contentCreator"].FirstOrDefault();

            if (!string.IsNullOrEmpty(notiRecieverId))
            {
                RegisterConnection(notiRecieverId, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            LiveDonationConnections.Remove(connectionId);
            return base.OnDisconnectedAsync(exception);
        }

        private void RegisterConnection(string userDonateId, string connectionId) => LiveDonationConnections.Add(userDonateId, connectionId);

    }
}
