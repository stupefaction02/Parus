using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Core.Network
{
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
        }

        // TODO: Result struct instead bool
        public async Task<RefreshTokenResult> RequestRefreshTokenAsync(string fingerprint, string refreshToken)
        {
            string path = $"?fingerprint={fingerprint}&refreshToken={refreshToken}";

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
                string res = await response.Content.ReadAsStringAsync();
                return new RefreshTokenResult(response.IsSuccessStatusCode);
            }

            return new RefreshTokenResult(response.IsSuccessStatusCode);
        }
    }
}
