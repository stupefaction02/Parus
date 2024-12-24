using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Parus.API;
using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.Identity;
using static Parus.API.CustomHttpContexts;

namespace Parus.Backend.Services.Chat.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserRepository users;

        public ChatHub(IAuthenticationService authenticationService, 
            IHttpContextAccessor httpContextAccessor,
            IUserRepository users)
        {
            this.authenticationService = authenticationService;
            this.httpContextAccessor = httpContextAccessor;
            this.users = users;
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
            string connString = Context.ConnectionId;

            ParusUser user;
            // checks if the user forgot to call JoinChat method
            // sort of a mean of defense :P
            if (!connectionsUsers.TryGetValue(connString, out user))
            {
                await SendUserNotFoundUnicast();
                return;
            }

            if (Context.User.Identity.IsAuthenticated)
            {
                string username = Context.User.Identity.Name;
                Console.WriteLine("ChatHub. " + username + ": " + message);
                await Clients.All.SendAsync("Receive", message, username, color);
            }
        }

        private async Task SendUserNotFoundUnicast()
        {
            var userNotFound = new { success = "false", errorCode = "USER_NOT_FOUND", statusCode = "404" };
            await Clients.Client(Context.ConnectionId).SendAsync("HandleErrors", userNotFound);
        }

        private async Task SendServerErrorUnicast()
        {
            var userNotFound = new { success = "false", errorCode = "SERVER_ERROR", statusCode = "500" };
            await Clients.Client(Context.ConnectionId).SendAsync("HandleErrors", userNotFound);
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

        private readonly Dictionary<string, ParusUser> connectionsUsers = new Dictionary<string, ParusUser>();

        public async Task JoinChat(string chatName, string authenticationValue)
        {
            // first level of defense
            if (String.IsNullOrEmpty(chatName) && String.IsNullOrEmpty(authenticationValue))
            {
                return;
            }

            HttpContext context = Context.GetHttpContext() ?? httpContextAccessor.HttpContext;
            context.Request.Headers.Authorization = $"Bearer {authenticationValue}";

            // second level of defense
            var authResult = await authenticationService.AuthenticateAsync(context, JwtBearerDefaults.AuthenticationScheme);

            if (!authResult.Succeeded)
            {
                return;
            }

            var connectedUser = users.One(x => x.GetUsername() == authResult.Principal.Identity.Name);

            // third level of defense
            if (connectedUser == null)
            {
                await SendUserNotFoundUnicast();
                return;
            }

            connectionsUsers.Add(Context.ConnectionId, (ParusUser)connectedUser);

            Console.WriteLine($"User conn.id={Context.ConnectionId} has joined {chatName} group");

            await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
        }
    }
}