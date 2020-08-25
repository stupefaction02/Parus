using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Naturistic.Core.Entities;
using Naturistic.Core.Extensions;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.WebUI.Services
{ 
    public class ApiClient : IApiClient
    {
        public static string BaseApiUri = "https://localhost:5001/api";

        private readonly HttpClient httpClient;

        public ApiClient(HttpClient client)
        {
            httpClient = client;
        }
		
		public async Task<object> RegisterAsync(string nickname, string firstname, string lastname, string email, string password)
		{
			Console.WriteLine("Register has begun...");

            string requestUri = $"{ApiClient.BaseApiUri}/register?firstname={firstname}" +
                $"&lastname={lastname}&nickname={nickname}" +
                $"&email={email}&password={password}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri)
            };

            return await httpClient.SendAsync(request);
        }
    }
}
