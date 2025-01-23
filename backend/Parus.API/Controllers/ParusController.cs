using Microsoft.AspNetCore.Mvc;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System.Security.Claims;
using Parus.API.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Parus.Core.Authentication;
using Parus.Infrastructure.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;

namespace Parus.API.Controllers
{
    public class ParusController : Controller
    {
        protected void LogInfo_Debug(string message)
        {
#if DEBUG
            Console.WriteLine(message);    
#endif
        }

        // TODO: Format appropriate to 'API Best Practices'
		protected JsonResult JsonSuccess()
		{
			return Json(new { success = "Y" });
		}

        // TODO: Format appropriate to 'API Best Practices'
        protected JsonResult JsonFail()
        {
            return Json(new { success = "N" });
        }

        protected JsonResult JsonFail(string errorMessage)
        {
            return Json(new { success = "N", error = errorMessage });
        }

        protected object HandleServerError(string serviceName, string debugInfo, object param = null, string returnMessage = "")
        {
            HttpContext.Response.StatusCode = 500;
            return "";
        }

		// TODO: Move to Parus.Core
		protected JwtToken CreateJWT(ClaimsIdentity user)
        {
			//logger.LogInformation($"Tryig to create JWT token for user {user.Name} ...");

			DateTime now = DateTime.UtcNow;

			JwtSecurityToken jwt = new JwtSecurityToken(
					issuer: JwtAuthOptions1.ISSUER,
					audience: JwtAuthOptions1.AUDIENCE,
					notBefore: now,
					claims: user.Claims,
					expires: now.Add(TimeSpan.FromMinutes(JwtAuthOptions1.LIFETIME)),
					signingCredentials: new SigningCredentials(JwtAuthOptions1.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            //logger.LogInformation(encodedJwt);

            //logger.LogInformation($"JWT token for user {user.Name} has been created.");

            return new JwtToken
            {
                Token = encodedJwt,
                Username = user.Name
			};
		}

        protected async Task<ClaimsIdentity> CreateIdentityAsync(ParusUser user)
        {
            List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.GetUsername())
                };

            return new ClaimsIdentity(claims);
        }

        // TODO: I don't know... instead of method, make a service or something 
        protected async Task<JsonResult> LoginResponse(ParusUser appUser)
        {
            ClaimsIdentity identity = await CreateIdentityAsync(appUser);

            JwtToken newToken = CreateJWT(identity);

            return Json(new
            {
                success = "true",
                payload = newToken.Token
            });
        }

        protected async Task<object> HandleLoginAsync(ParusUser user, string fingerPrint, ParusDbContext dbContext)
        {
            //var existedRt = dbContext.RefreshSessions.FirstOrDefault(x => x.Fingerprint == fingerPrint && x.UserId == user.GetId());
            var existedRt = dbContext.RefreshSessions.FirstOrDefault(x => x.UserId == user.GetId());

            if (existedRt == null)
            {
                return HandleServerError("Identity", "");
            }

            existedRt.Token = RefreshSession.GenerateToken();

            var updateRT = dbContext.Update(existedRt);

            if (await dbContext.SaveChangesAsync() <= 0)
            {
                return Json(HandleServerError("Identity", "", "Server Error. Contact the administrator."));
            }

            Console.WriteLine($"RefreshSession was updated. User: {user.UserName} , Token: {existedRt.Token}");

            ClaimsIdentity identity = await CreateIdentityAsync(user);

            JwtToken newToken = CreateJWT(identity);

            return Json(new
            {
                success = "true",
                payload = newToken.Token,
                refreshToken = updateRT.Entity.Token
            });
        }

        protected string GenerateJwtForUser(string username)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, username)
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims);

            return CreateJWT(identity).Token;
        }
    }

    public static class HostingExtensions
    {
        public static bool IsAnyDevelopment(this IHostEnvironment env)
        {
            if (env.IsEnvironment("Development_Localhost"))
            {
                return true;
            }
            else if (env.IsDevelopment())
            {
                return true;
            }

            return false;
        }
    }
}
