using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Naturistic.Core.Interfaces;
using Naturistic.Core.Services.Localization;

namespace Naturistic.WebUI.Middlewares
{
	public class LocalizationMiddleware
	{
        private readonly RequestDelegate _next;
		private readonly IServiceProvider serviceProvider;
        private readonly ILogger<DebugMiddleware> logger;

        public LocalizationMiddleware(RequestDelegate next, IServiceProvider serviceProvider, ILogger<DebugMiddleware> logger)
        {
            _next = next;
			this.serviceProvider = serviceProvider;
            this.logger = logger;
        }

		public Task Invoke(HttpContext httpContext)
		{
			string locale = httpContext.Request.Cookies["locale"];

			if (String.IsNullOrEmpty(locale))
			{
				// TODO: locale locale by ip
				httpContext.Response.Cookies.Append("locale", locale = "ru");
			}

			return _next(httpContext);
		}
	}
}