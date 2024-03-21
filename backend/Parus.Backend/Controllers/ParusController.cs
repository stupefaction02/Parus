using Microsoft.AspNetCore.Mvc;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System.Security.Claims;
using Parus.Backend.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Parus.Core.Authentication;
using Parus.Infrastructure.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Parus.Backend.Controllers
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
            return StatusCode(500);
        }

		// TODO: Move to Parus.Core
		protected JwtToken CreateJWT(ClaimsIdentity user)
        {
			//logger.LogInformation($"Tryig to create JWT token for user {user.Name} ...");

			DateTime now = DateTime.UtcNow;

			JwtSecurityToken jwt = new JwtSecurityToken(
					issuer: JwtAuthOptions.ISSUER,
					audience: JwtAuthOptions.AUDIENCE,
					notBefore: now,
					claims: user.Claims,
					expires: now.Add(TimeSpan.FromMinutes(JwtAuthOptions.LIFETIME)),
					signingCredentials: new SigningCredentials(JwtAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            //logger.LogInformation(encodedJwt);

            //logger.LogInformation($"JWT token for user {user.Name} has been created.");

            return new JwtToken
            {
                Token = encodedJwt,
                Username = user.Name
			};
		}

        protected async Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user)
        {
            List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.GetUsername())
                };

            return new ClaimsIdentity(claims);
        }

        // TODO: I don't know... instead of method make a service or something 
        protected async Task<JsonResult> LoginResponse(ApplicationUser appUser)
        {
            ClaimsIdentity identity = await CreateIdentityAsync(appUser);

            JwtToken newToken = CreateJWT(identity);

            return Json(new
            {
                success = "true",
                payload = newToken.Token
            });
        }

        protected async Task<JsonResult> LoginResponse(ApplicationUser user, string fingerPrint, ApplicationIdentityDbContext dbContext)
        {
            var existedRt = dbContext.RefreshSessions.FirstOrDefault(x => x.Fingerprint == fingerPrint & x.User == user);

            if (existedRt == null)
            {
                return Json( HandleServerError("Identity", "") );
            }

            existedRt.Token = RefreshSession.GenerateToken();

            var updateRT = dbContext.Update(existedRt);

            if (await dbContext.SaveChangesAsync() <= 0)
            {
                return Json(HandleServerError("Identity", "", "Server Error. Contact the administrator."));
            }

            ClaimsIdentity identity = await CreateIdentityAsync(user);

            JwtToken newToken = CreateJWT(identity);

            return Json(new
            {
                success = "true",
                payload = newToken.Token,
                refreshToken = updateRT.Entity.Token
            });
        }
    }
}
