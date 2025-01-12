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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Parus.Infrastructure.Identity;
using Parus.Infrastructure.Services;
using static System.Formats.Asn1.AsnWriter;

namespace Parus.Backend.Middlewares
{
	public class ParusAuthenticateFeatures : IAuthenticateResultFeature, IHttpAuthenticationFeature
	{
		private ClaimsPrincipal? _user;
		private AuthenticateResult? _result;

		public ParusAuthenticateFeatures(AuthenticateResult result)
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

	public class ParusAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RefreshTokensService refreshTokens;
        private readonly ILogger<ParusAuthenticationMiddleware> logger;

        public ParusAuthenticationMiddleware(RequestDelegate next, RefreshTokensService refreshTokens)
        {
            _next = next;
            this.refreshTokens = refreshTokens;
        }

		public async Task Invoke(HttpContext httpContext)
		{
			// If standart cookie authentcation has failed (generally due of abstance of login cookie)
			if (!httpContext.User.Identity.IsAuthenticated)
			{
                var headers = httpContext.Request.Headers;
                StringValues token;
                if (!headers.TryGetValue("Authorization", out token))
                {
                    string jwtCoockie = httpContext.Request.Cookies["JWT"];

                    if (!String.IsNullOrEmpty(jwtCoockie))
                    {
                        if (headers.ContainsKey("Authorization"))
                        {
                            headers["Authorization"] = "Bearer " + jwtCoockie;
                        }
                        else
                        {
                            headers.Add("Authorization", "Bearer " + jwtCoockie);
                        }

                        await Authenticate(httpContext);
                    }
                }
                else
                {
                    if (token.Count > 0)
                    {
                        await Authenticate(httpContext);
                    }
                    else
                    {
                        string jwtCoockie = httpContext.Request.Cookies["JWT"];

                        if (!String.IsNullOrEmpty(jwtCoockie))
                        {
                            if (headers.ContainsKey("Authorization"))
                            {
                                headers["Authorization"] = "Bearer " + jwtCoockie;
                            }
                            else
                            {
                                headers.Add("Authorization", "Bearer " + jwtCoockie);
                            }

                            await Authenticate(httpContext);
                        }
                    }
                }
            }

			await _next(httpContext);
		}

        private async Task Authenticate(HttpContext httpContext)
        {
            AuthenticateResult result = await httpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);

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

                ParusAuthenticateFeatures authFeatures = new ParusAuthenticateFeatures(result);
                httpContext.Features.Set<IHttpAuthenticationFeature>(authFeatures);
                httpContext.Features.Set<IAuthenticateResultFeature>(authFeatures);
            }
            else
            {
                if (result.Failure != null)
                {
                    string error = result.Failure.Message;

                    // token is expired
                    if (result.Failure is SecurityTokenSignatureKeyNotFoundException)
                    {
                        string fingerPrint = httpContext.Request.Cookies["fingerprint"];
                        //refreshTokens.GetSession(fingerPrint);

                        httpContext.Response.Headers.Append("Set-Cookie", $"JWT='123';");
                    }
                }
                //logger.LogError(error);
            }
        }
    }
}