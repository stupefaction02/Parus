using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Naturistic.Infrastructure.Identity;
using static System.Formats.Asn1.AsnWriter;

namespace Naturistic.WebUI.Middlewares
{
    public class CheckingLoggingInMiddleware
    {
        private readonly RequestDelegate _next;
		private readonly IServiceProvider serviceProvider;

		public CheckingLoggingInMiddleware(RequestDelegate next, IServiceProvider servicePRovider)
        {
            _next = next;
			this.serviceProvider = servicePRovider;
		}

		public Task Invoke(HttpContext httpContext)
		{
			if (!httpContext.User.Identity.IsAuthenticated)
			{
				string jwtCoockie = httpContext.Request.Cookies["JWT"];

				if (!String.IsNullOrEmpty(jwtCoockie))
				{
					httpContext.Request.Headers.Add("Authorization", "Bearer " + jwtCoockie);

					//if (result?.Succeeded ?? false)
					//{
					//	var authFeatures = new AuthenticationFeatures(result);
					//	httpContext.Features.Set<IHttpAuthenticationFeature>(authFeatures);
					//	httpContext.Features.Set<IAuthenticateResultFeature>(authFeatures);
					//}

					using (IServiceScope scope = serviceProvider.CreateScope())
					{
						IAuthenticationService authService = scope.ServiceProvider.GetRequiredService<IAuthenticationService>();
						var um = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

						var result = (authService.AuthenticateAsync(httpContext, JwtBearerDefaults.AuthenticationScheme)).Result;

						if (result?.Principal != null)
						{
							httpContext.User = result.Principal;

							var t = (um.GetUserAsync(result.Principal)).Result;


						}
					}
				}
			}

			return _next(httpContext);
		}
	}
}