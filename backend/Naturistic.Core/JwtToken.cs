using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Naturistic.Core
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


}
