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
	public class RestrictConnectionsMiddleware
    {
        private readonly RequestDelegate _next;
		private readonly IServiceProvider serviceProvider;

        public RestrictConnectionsMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<DebugMiddleware> logger)
        {
            _next = next;
			this.serviceProvider = serviceProvider;
        }

        // TODO: pull from config
        string debugToken = "f73fcd367f14e96a47d603892fe51ded";

        public Task Invoke(HttpContext httpContext)
		{
            string debugToken = httpContext.Request.Cookies["DEBUG_TOKEN"];

            if (!String.IsNullOrEmpty(debugToken))
            {
                if (debugToken == this.debugToken)
                {
                    return _next(httpContext);
                }
            }
            httpContext.Response.StatusCode = 403;
            httpContext.Abort();
            return _next(httpContext);
        }

    }
}