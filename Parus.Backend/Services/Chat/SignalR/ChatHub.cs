using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Parus.Backend.Services.Chat.SignalR
{
    public class ChatHub : Hub
    {
        public ChatHub()
        {
            
        }

        public async Task Send(string message, string color, string chatName)
        {
            if (Context.User.Identity.IsAuthenticated)
            { 
                string username = Context.User.Identity.Name;
                Console.WriteLine("ChatHub. " + username + ": " + message);
                await Clients.Group(chatName).SendAsync("Receive", message, username, color);
            }
        }

        public async Task SendWithChatRecord(string message, string color)
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
            //string chatName = Context.GetHttpContext().Request.Cookies["chatName"];
            //Console.WriteLine(Context.ConnectionId);
            //Groups.AddToGroupAsync(Context.ConnectionId, chatName);
            
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {

            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChat(string chatName)
        {
            Console.WriteLine($"User conn.id={Context.ConnectionId} has joined {chatName} group");
            await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
        }
    }
}