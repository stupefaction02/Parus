using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Naturistic.Backend.Services.Chat.SignalR
{
    public class ChatHub : Hub
    {
        public async Task Send(string message, string nickname)
        {
            await this.Clients.All.SendAsync("Receive", message, nickname);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        } 
    }
}