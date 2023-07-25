using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Naturistic.WebUI.Middlewares
{
    public class CheckingLoggingInMiddleware
    {
        private readonly RequestDelegate _next;
		private readonly IAuthenticationService authenticationService;

		public CheckingLoggingInMiddleware(RequestDelegate next, IAuthenticationService authenticationService)
           {
              _next = next;
			this.authenticationService = authenticationService;
		}
        
           public Task Invoke(HttpContext httpContext)
           {
				return _next(httpContext);
           }
    }
}