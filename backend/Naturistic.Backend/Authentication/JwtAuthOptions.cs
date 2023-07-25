using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Text.Json.Serialization;

namespace Naturistic.Backend.Authentication
{
	public class JwtAuthOptions
	{
		public const string ISSUER = "https://localhost:5000";
		public const string AUDIENCE = "https://localhost:5002";
		const string KEY = "mysupersecret_secretkey!123"; 
		public const int LIFETIME = 60 * 24 * 3;
		public static SymmetricSecurityKey GetSymmetricSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
		}
	}
}