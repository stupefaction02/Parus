using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Naturistic.Infrastructure.Identity;
using static System.Formats.Asn1.AsnWriter;

namespace Naturistic.Backend.Middlewares
{
	public class NaturisticAuthenticateFeatures : IAuthenticateResultFeature, IHttpAuthenticationFeature
	{
		private ClaimsPrincipal? _user;
		private AuthenticateResult? _result;

		public NaturisticAuthenticateFeatures(AuthenticateResult result)
		{
			AuthenticateResult = result;
		}

		public AuthenticateResult? AuthenticateResult
		{
			get => _result;
			set
			{
				_result = value;
				_user = _result?.Principal;
			}
		}

		public ClaimsPrincipal? User
		{
			get => _user;
			set
			{
				_user = value;
				_result = null;
			}
		}
	}

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
			// If standart cookie authentcation has failed (generally due of abstance of login cookie)
			if (!httpContext.User.Identity.IsAuthenticated)
			{
                StringValues token;
                if (!httpContext.Request.Headers.TryGetValue("Authorization", out token))
                {
                    string jwtCoockie = httpContext.Request.Cookies["JWT"];

                    if (!String.IsNullOrEmpty(jwtCoockie))
                    {
                        httpContext.Request.Headers.Add("Authorization", "Bearer " + jwtCoockie);

                        Authenticate(httpContext);
                    }
                }
                else
                {
                    if (token.Count > 0)
                    {
                        Authenticate(httpContext);
                    }
                    else
                    {
                        string jwtCoockie = httpContext.Request.Cookies["JWT"];

                        if (!String.IsNullOrEmpty(jwtCoockie))
                        {
                            httpContext.Request.Headers.Add("Authorization", "Bearer " + jwtCoockie);

                            Authenticate(httpContext);
                        }
                    }
                }
            }

			return _next(httpContext);
		}

        private void Authenticate(HttpContext httpContext)
        {
            AuthenticateResult result = (httpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme)).Result;

            if (result.Succeeded)
            {
                CookieOptions cookieOptions = new CookieOptions() { Path = "/" };

                httpContext.Response.Cookies.Append("identity.username", result.Principal.Identity.Name, cookieOptions);
                // in any case
                httpContext.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
                {
                    OriginalPath = httpContext.Request.Path,
                    OriginalPathBase = httpContext.Request.PathBase
                });

                httpContext.User = result.Principal;

                NaturisticAuthenticateFeatures authFeatures = new NaturisticAuthenticateFeatures(result);
                httpContext.Features.Set<IHttpAuthenticationFeature>(authFeatures);
                httpContext.Features.Set<IAuthenticateResultFeature>(authFeatures);
            }
        }
    }
}