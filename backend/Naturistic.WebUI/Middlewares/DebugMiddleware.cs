using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Parus.Infrastructure.Identity;
using static System.Formats.Asn1.AsnWriter;

namespace Parus.WebUI.Middlewares
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

		public Task Invoke(HttpContext httpContext)
		{
            ClaimsPrincipal user = httpContext.User;
			bool authenticated = user.Identity.IsAuthenticated;

            UserManager<ApplicationUser> userManager = default(UserManager<ApplicationUser>);

            string userInfo = "none";
            string rolesInfo = "none";
            if (user.Identity.IsAuthenticated)
			{
                userInfo = user.Identity.Name;
                using (var scope = serviceProvider.CreateScope())
                {
                    userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    if (userManager == null)
                    {
                        throw new Exception("UserManger is not set.");
                    }

                    ApplicationUser user1 = Task.Run<ApplicationUser>
                        (async () => await userManager.FindByNameAsync(user.Identity.Name)).Result;

                    if (user1 != null)
                    {
                        IList<string> roles = Task.Run<IList<string>>
                        (async () => await userManager.GetRolesAsync(user1)).Result;

                        if (roles.Count > 0)
                        {
                            foreach (var role in roles)
                            {
                                rolesInfo += role + " ";
                            }
                        }
                    }
                }
            }
            
            string info = $"User: {userInfo}, Authenticated: {authenticated}, Roles: {rolesInfo}";

			Debug.WriteLine(info);
			logger.LogInformation(info);

			return _next(httpContext);
		}
	}
}