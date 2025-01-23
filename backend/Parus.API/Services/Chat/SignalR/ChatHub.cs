using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Loggers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Parus.API;
using Parus.API.Services;
using Parus.Core.Interfaces.Repositories;
using Parus.Infrastructure.Identity;
using static Parus.API.CustomHttpContexts;

namespace Parus.API.Services.Chat.SignalR
{
    public class ChatHub : Hub
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserRepository users;
        private readonly ILogger<ChatHub> logger;
        private readonly SharedChatAuthenticatedUsers sharedAuthenticatedUsers;

        public ChatHub(IAuthenticationService authenticationService, 
            IHttpContextAccessor httpContextAccessor,
            IUserRepository users,
            SharedChatAuthenticatedUsers sharedAuthenticatedUsers, 
            ILogger<ChatHub> logger)
        {
            this.logger = logger;
            this.authenticationService = authenticationService;
            this.httpContextAccessor = httpContextAccessor;
            this.users = users;
            this.sharedAuthenticatedUsers = sharedAuthenticatedUsers;
        }

        public async Task Send(string message, string color, string chatName)
        {
            string connString = Context.ConnectionId;

            ParusUser user;
            // checks if the user forgot to call JoinChat method
            // sort of a mean of defense :P
            if (!sharedAuthenticatedUsers.TryGet(connString, out user))
            {
                await SendErrorUnicast("USER_NOT_FOUND", "500");
                return;
            }
            
            string username = user.UserName;
            logger.LogInformation($"ChatHub. {username} sent in chat {chatName} the message: \"{message}\"");
            await Clients.Group(chatName).SendAsync("Receive", message, username, color);
        }

        public async Task SendWithChatRecord(string message, string color)
        {
            string connString = Context.ConnectionId;

            ParusUser user;
            // checks if the user forgot to call JoinChat method
            // sort of a mean of defense :P
            if (!connectionsUsers.TryGetValue(connString, out user))
            {
                //await SendUserNotFoundUnicast();
                return;
            }

            if (Context.User.Identity.IsAuthenticated)
            {
                string username = Context.User.Identity.Name;
                Console.WriteLine("ChatHub. " + username + ": " + message);
                await Clients.All.SendAsync("Receive", message, username, color);
            }
        }

        public override async Task OnConnectedAsync()
        {
            string connString = Context.ConnectionId;

            HttpContext context = Context.GetHttpContext() ?? httpContextAccessor.HttpContext;

            string authValue = "";
            if (context.Request.Query.TryGetValue("access_token", out StringValues accessToken))
            {
                authValue = accessToken[0];
            }
            else if (context.Request.Headers.TryGetValue("Authorization", out StringValues token))
            {
                if (token.Count > 0)
                {
                    authValue = token[0];
                }
            }

            context.Request.Headers.Authorization = authValue;

            var authResult = await authenticationService.AuthenticateAsync(context, JwtBearerDefaults.AuthenticationScheme);

            if (authResult.Succeeded)
            {
                ParusUser connectedUser = users.One(x => x.GetUsername() == authResult.Principal.Identity.Name) as ParusUser;

                if (connectedUser != null)
                {
                    sharedAuthenticatedUsers.Set(Context.ConnectionId, connectedUser);
                    logger.LogInformation($"ChatHub. User with conn.id={Context.ConnectionId} and username={connectedUser.UserName} connected to the hub.");
                }
            }
            else
            {
                logger.LogInformation($"ChatHub. User with conn.id={Context.ConnectionId} connected to the hub.");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string connString = Context.ConnectionId;

            ParusUser disconnectedUser;
            sharedAuthenticatedUsers.Unset(connString, out disconnectedUser);

            await base.OnDisconnectedAsync(exception);
        }

        private async Task SendErrorUnicast(string errorCode, string statusCode, string message = "")
        {
            var error = new { success = "false", errorCode = errorCode, statusCode = statusCode };
            await Clients.Client(Context.ConnectionId).SendAsync("HandleErrors", error);
        }

        private readonly Dictionary<string, ParusUser> connectionsUsers = new Dictionary<string, ParusUser>();

        public async Task JoinChat(string chatName)
        {
            // first level of defense
            if (String.IsNullOrEmpty(chatName))
            {
                await SendErrorUnicast("BAD_REQUEST", "403");
                return;
            }

            string connString = Context.ConnectionId;

#if DEBUG
            ParusUser connectedUser;
            if (sharedAuthenticatedUsers.TryGet(connString, out connectedUser))
            {
                logger.LogInformation($"ChatHub. User conn.id={Context.ConnectionId}, username={connectedUser.UserName} has joined {chatName} group");
            } 
            else 
            {
                logger.LogInformation($"ChatHub. User conn.id={Context.ConnectionId} has joined {chatName} group");
            }
            
#else
            Console.WriteLine($"ChatHub. User conn.id={Context.ConnectionId} has joined {chatName} group");
#endif
            await Groups.AddToGroupAsync(Context.ConnectionId, chatName);
        }
    }
}