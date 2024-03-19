using System;
using System.Diagnostics;
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
using static System.Formats.Asn1.AsnWriter;

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

	public class IdentityHttpClient : HttpClient
	{
        private readonly string refreshTokenUrl = "api/account/refreshtoken";

        private readonly string apiToken;

        public IdentityHttpClient(string url, string apiToken)
        {
			BaseAddress = new Uri(url);
            this.apiToken = apiToken;
        }

		// TODO: Result struct instead bool
		public async Task<bool> RequestRefreshTokenAsync(string fingerprint, string refreshToken)
		{
			string path = $"?fingerprint={fingerprint}&refreshToken={refreshToken}";

            HttpRequestMessage request = new HttpRequestMessage
			{
				RequestUri = new Uri(BaseAddress + refreshTokenUrl + path)
			};

			Console.WriteLine(request.RequestUri);

			var response = await SendAsync(request);

            Console.WriteLine(response.StatusCode);

            return response.IsSuccessStatusCode;
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
					httpContext.Request.Headers.Add("Authorization", "Bearer " + jwtCoockie);

                    // if failes with method not found
                    // https://stackoverflow.com/questions/76750686/method-not-found-boolean-microsoft-identitymodel-tokens-tokenutilities-isrecov
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
					else
					{
                        //result.Failure

						// TODO: VS2022 doens't see public exception class
                        //https://source.dot.net/#Microsoft.AspNetCore.Authentication.Abstractions/AuthenticationFailureException.cs,c7780b6f21f367ad,references
                        //if (result.Failure is AuthenticationFailureException)
						
						if (result.Failure.Message.StartsWith("IDX10223"))
						{
							string fingerprint = httpContext.Request.Cookies["fingerprint"];
							string refreshToken = "95d453f41f074b3b96f9a73d4e5cb5df";// httpContext.Request.Cookies["refreshToken"];
							if (await httpClient.RequestRefreshTokenAsync(fingerprint, refreshToken))
							{
								Debug.WriteLine("Refresh Token request has completed!");
							}                   
                        }
                    }
				}
			}

			await _next(httpContext);
		}
	}
}