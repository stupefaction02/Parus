using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Parus.Infrastructure.Identity;
using static System.Formats.Asn1.AsnWriter;

namespace Parus.Infrastructure.Middlewares
{
    public class DebugMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<DebugMiddleware> logger;

        public DebugMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<DebugMiddleware> logger)
        {
            _next = next;
            this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            ClaimsPrincipal user = httpContext.User;
            bool authenticated = user.Identity.IsAuthenticated;

            UserManager<ParusUser> userManager = default(UserManager<ParusUser>);

            string userInfo = "none";
            string rolesInfo = "none";
            if (user.Identity.IsAuthenticated)
            {
                userInfo = user.Identity.Name;
                using (var scope = serviceProvider.CreateScope())
                {
                    userManager = scope.ServiceProvider.GetRequiredService<UserManager<ParusUser>>();

                    if (userManager == null)
                    {
                        throw new Exception("UserManger is not set.");
                    }

                    ParusUser parusUser = await userManager.FindByNameAsync(user.Identity.Name);

                    if (user != null)
                    {
                        IList<string> roles = await userManager.GetRolesAsync(parusUser);

                        if (roles.Count > 0)
                        {
                            rolesInfo = "";
                            foreach (var role in roles)
                            {
                                rolesInfo += role + " ";
                            }

                            rolesInfo.TrimEnd();
                        }
                    }
                }
            }

            string time = DateTime.UtcNow.ToString("HH:mm:ss");

            string info = $"[{time}] User: {userInfo}, Authenticated: {authenticated}, Roles: {rolesInfo}. Path: {httpContext.Request.Path}. Connection: {httpContext.Connection.RemoteIpAddress}:{httpContext.Connection.RemotePort}";

            Debug.WriteLine(info);
            logger.LogInformation(info);

            await _next(httpContext);
        }
    }
}