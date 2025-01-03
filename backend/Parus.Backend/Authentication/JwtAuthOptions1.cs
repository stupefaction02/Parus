using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Parus.Core;
using Parus.Core.Authentication;
using Parus.Infrastructure.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Parus.Backend.Authentication
{
	public class JwtAuthOptions
	{
        public string SecretKey { get; set; } = "<!{_Secr-<>-etKey!{_>";

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int LifetimeMinutes { get; set; } 
    }

	public static class JwtAuthUtils
	{
		public static JwtToken One(ClaimsIdentity user, JwtAuthOptions options)
        {
            DateTime now = DateTime.UtcNow;

            // options.PrivateKey is "" by default, so no null exceptions here
            var securityKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(options.SecretKey));

            JwtSecurityToken jwt = new JwtSecurityToken(
                    issuer: options.Issuer,
                    audience: options.Audience,
                    notBefore: now,
                    claims: user.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(options.LifetimeMinutes)),
                    signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256));
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new JwtToken
            {
                Token = encodedJwt,
                Username = user.Name
            };
        }
	}

    public class JwtAuthOptions1
	{
		public const string ISSUER = "https://localhost:5002";
		public const string AUDIENCE = "https://localhost:5002";
		const string KEY = "{amogus!1000!}{zzyzzyy}1234567890!ilovejwttokenssomuchitsunreal!forreal1290!}}}!asdewegwg!!!12!!!!}{}{}"; 
		// in minutes
		public const int LIFETIME = 60 * 24 * 3 * 7 * 31;
		//public const int LIFETIME = 1;

		public static TimeSpan Lifetime;

		public static SymmetricSecurityKey GetSymmetricSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.Default.GetBytes(KEY));
		}
	}
}