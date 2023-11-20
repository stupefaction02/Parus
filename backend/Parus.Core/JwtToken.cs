using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Parus.Core
{

	[Serializable]
	public class JwtToken
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }

		[JsonPropertyName("username")]
		public string Username { get; set; }
	}

    [Serializable]
    public class ApiServerResponse
    {
        [JsonPropertyName("success")]
        public string Success { get; set; }

        [JsonPropertyName("payload")]
        public object Payload { get; set; }
    }

    [Serializable]
    public class AuthenticateResponseJson
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("access_token")]
        public JwtTokenJson AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public RefreshTokenJson RefreshToken { get; set; }
    }

    [Serializable]
    public class JwtTokenJson
    {
        [JsonPropertyName("jwt")]
        public string Token { get; set; }

        [JsonPropertyName("expires")]
        public object ExpiresAt { get; set; }
    }

    [Serializable]
    public class RefreshTokenJson
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("expires")]
        public object ExpiresAt { get; set; }
    }
}
