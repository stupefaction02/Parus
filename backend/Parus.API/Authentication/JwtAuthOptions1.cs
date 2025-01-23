using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Parus.Core;
using Parus.Core.Authentication;
using Parus.Core.Identity;
using Parus.Infrastructure.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Parus.API.Authentication
{
	

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