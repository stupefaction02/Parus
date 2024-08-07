using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Parus.Core.Network
{
    public class RefreshTokenResultJson
    {
        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }
    }

    public struct RefreshTokenResult
    {
        public bool Success { get; set; }

        public RefreshTokenResult(bool success, string refreshToken = "", string accessToken = "") : this()
        {
            Success = success;
            RefreshToken = refreshToken;
            AccessToken = accessToken;
        }

        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }
    }

    public class IdentityHttpClient : HttpClient
    {
        private readonly string refreshTokenUrl = "api/account/refreshtoken";

        private readonly string apiToken;

        public IdentityHttpClient(string url, string apiToken)
        {
            BaseAddress = new Uri(url);
            this.apiToken = apiToken;

            BaseAddress = new Uri("https://paruseatingnuts.duckdns.org:39003");
        }

        public async Task<RefreshTokenResult> RequestRefreshTokenAsync(string fingerprint, string refreshToken)
        {
            string path = $"?fingerPrint={fingerprint}&refreshToken={refreshToken}";

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(BaseAddress + refreshTokenUrl + path)
            };

            Console.WriteLine(request.RequestUri);

            var response = await SendAsync(request);

            Console.WriteLine(response.StatusCode);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                RefreshTokenResultJson r;
                using (MemoryStream ms = new MemoryStream())
                {
                    r = System.Text.Json.JsonSerializer.Deserialize<RefreshTokenResultJson>(responseString);
                }

                return new RefreshTokenResult(response.IsSuccessStatusCode, r.RefreshToken, r.AccessToken);
            }

            return new RefreshTokenResult(response.IsSuccessStatusCode);
        }
    }
}
