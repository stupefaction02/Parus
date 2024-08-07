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

                // sources ^)
                //https://source.dot.net/Microsoft.AspNetCore.SignalR.Core/R/946e904ad0c29cfa.html
                //https://source.dot.net/Microsoft.AspNetCore.SignalR.Core/R/620f216b8183de98.html
                //https://source.dot.net/Microsoft.AspNetCore.SignalR.Core/R/316a8601722cf9bd.html
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
            if (String.IsNullOrEmpty(chatName))
            {
                return;
            }

            Console.WriteLine($"User conn.id={Context.ConnectionId} has joined {chatName} group");
            await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
        }
    }
}