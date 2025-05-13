using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace CabPostService.EventHub.SignalRHub
{
    public class DonateHub : Hub
    {
        public async Task SendDonateNotification(string method, string message)
        {
            await Clients.Caller.SendAsync(method, message);//"ReceiveDonateNotification"
        }
    }
}
