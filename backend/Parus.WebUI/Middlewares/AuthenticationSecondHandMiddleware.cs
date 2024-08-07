using System;
using System.Diagnostics;
using System.IO;
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
using Parus.Infrastructure.Identity;
using Parus.Core.Network;
using static System.Formats.Asn1.AsnWriter;
using Parus.Core.Interfaces.Services;
using System.Collections.Generic;

namespace Parus.WebUI.Middlewares
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

    public class AuthenticationSecondHandMiddleware
    {
        private readonly RequestDelegate _next;
		private readonly IServiceProvider serviceProvider;

		public AuthenticationSecondHandMiddleware(RequestDelegate next, IServiceProvider servicePRovider)
        {
            _next = next;
			this.serviceProvider = servicePRovider;
		}

		public async Task Invoke(HttpContext httpContext, IdentityHttpClient httpClient)
		{
            // If standart cookie authentcation ( UseAuthentication() should be above this mw ) has failed (generally due of abstance of login cookie)
            if (!httpContext.User.Identity.IsAuthenticated)
			{
				string jwtCoockie = httpContext.Request.Cookies["JWT"];

				if (!String.IsNullOrEmpty(jwtCoockie))
				{
                    var headers = httpContext.Request.Headers;

                    if (!headers.TryAdd("Authorization", "Bearer " + jwtCoockie))
                    {
                        headers.Authorization = "Bearer " + jwtCoockie;
                    }

                    // if failes with method not found
                    // https://stackoverflow.com/questions/76750686/method-not-found-boolean-microsoft-identitymodel-tokens-tokenutilities-isrecov
                    AuthenticateResult result = (httpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme)).Result;

					if (result.Succeeded)
					{
                        OnSuccessAuthentication(httpContext, result);
					} 
					else
					{
						//if (mQService.GetRefreshToken())
						//{

						//}

                        //await _next(httpContext); return;
                        // TODO: VS2022 doens't see public exception class
                        //https://source.dot.net/#Microsoft.AspNetCore.Authentication.Abstractions/AuthenticationFailureException.cs,c7780b6f21f367ad,references
                        //if (result.Failure is AuthenticationFailureException)
                        if (result.Failure.Message.StartsWith("IDX10223"))
						{
							string fingerprint = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:123.0) Gecko/20100101 Firefox/123.0";//httpContext.Request.Cookies["fingerprint"];
							string refreshToken = httpContext.Request.Cookies["refreshToken"];
							RefreshTokenResult rtTokenRequestResult = await httpClient.RequestRefreshTokenAsync(fingerprint, refreshToken);

							if (rtTokenRequestResult.Success)
							{
								Console.WriteLine($"RequestRefreshToken is done. Token: {rtTokenRequestResult.RefreshToken}. Access token: {rtTokenRequestResult.AccessToken}");

                                if (!headers.TryAdd("Authorization", "Bearer " + rtTokenRequestResult.AccessToken))
                                {
                                    headers.Authorization = "Bearer " + rtTokenRequestResult.AccessToken;
                                }

                                AuthenticateResult secondAuthResult = (httpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme)).Result;

                                if (secondAuthResult.Succeeded)
                                {
                                    //httpContext.Response.Cookies.Append("refreshToken", rtTokenRequestResult.RefreshToken);

                                    //httpContext.Response.Headers.Append("Set-Cookie", $"refreshToken='{rtTokenRequestResult.RefreshToken}';");

                                    //OnSuccessAuthentication(httpContext, result);
                                }

                                httpContext.Response.Headers.Append("Set-Cookie", $"refreshToken='{rtTokenRequestResult.RefreshToken}';");

                                OnSuccessAuthentication(httpContext, result);
                            }
							else
							{
                                //httpContext.Response.Cookies.Delete("JWT");
                            }
                        }
						else
						{
							//httpContext.Response.Cookies.Delete("JWT");
						}
                    }
				}
			}

			await _next(httpContext);
		}

        private void OnSuccessAuthentication(HttpContext httpContext, AuthenticateResult result)
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