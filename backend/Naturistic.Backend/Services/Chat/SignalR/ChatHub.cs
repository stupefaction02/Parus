using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Naturistic.Backend.Services.Chat.SignalR
{
    public class ChatHub : Hub
    {
        public async Task Send(string message)
        {
            await this.Clients.All.SendAsync("ReceiveChatMessage", message);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        } 
    }
}