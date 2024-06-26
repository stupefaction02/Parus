using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Parus.Backend.Authentication
{
	public class JwtAuthOptions
	{
		public const string ISSUER = "https://localhost:5002";
		public const string AUDIENCE = "https://localhost:5002";
		const string KEY = "{amogus!1000!}{zzyzzyy}1234567890!ilovejwttokenssomuchitsunreal!forreal1290!}}}!asdewegwg!!!12!!!!}{}{}"; 
		// in minutes
		//public const int LIFETIME = 60 * 24 * 3 * 7 * 31;
		public const int LIFETIME = 1;

		public static TimeSpan Lifetime;

		public static SymmetricSecurityKey GetSymmetricSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.Default.GetBytes(KEY));
		}
	}
}