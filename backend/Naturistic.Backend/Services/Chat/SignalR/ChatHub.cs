using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Naturistic.Backend.Services.Chat.SignalR
{
    public class ChatHub : Hub
    {
        public async Task Send(string message)
        {
            Console.WriteLine("SignalR: " + message);
            await this.Clients.All.SendAsync("Receive", message);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        } 
    }
}