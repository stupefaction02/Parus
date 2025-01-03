using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Parus.Core
{

	[Serializable]
	public class JwtTokenJsonDTO
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }

		[JsonPropertyName("username")]
		public string Username { get; set; }
	}
}

namespace Parus.Core.Authentication
{
    public struct JwtToken
    {
        public string Token { get; set; }
        public string Username { get; set; }
    }
}
