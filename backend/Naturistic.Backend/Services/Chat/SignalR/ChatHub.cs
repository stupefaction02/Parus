using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Naturistic.Backend.Services.Chat.SignalR
{
    public class ChatHub : Hub
    {
        public ChatHub()
        {
            
        }

        public async Task Send(string message, string color)
        {
            if (Context.User.Identity.IsAuthenticated)
            { 
                string username = Context.User.Identity.Name;
                Console.WriteLine("ChatHub. " + username + ": " + message);
                await this.Clients.All.SendAsync("Receive", message, username, color);
            }
        }

		public override Task OnConnectedAsync()
        {
            Console.WriteLine("New user!");
            return base.OnConnectedAsync();
        } 
    }
}