using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using Parus.Core.Authentication;
using Parus.Core.Identity;
using Parus.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Parus.Infrastructure.Identity
{
    public static class JwtAuthUtils
    {
        public static JwtToken CreateDefault(ClaimsIdentity user, JwtAuthOptions options)
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
}

namespace Parus.Infrastructure.Extensions
{
    public static class ParusUserExtensions
    {
        public static ClaimsIdentity BuildIdentity(this ParusUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.GetUsername())
            };

            return new ClaimsIdentity(claims);
        }

        public static JwtToken JwtTokenFromUser(this ParusUser user, JwtAuthOptions authOptions) 
            => JwtAuthUtils.CreateDefault(user.BuildIdentity(), authOptions);
    }
}
