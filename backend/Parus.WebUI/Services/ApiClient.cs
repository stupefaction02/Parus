﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Parus.Core.Entities;
using Parus.Core.Extensions;
using Parus.Infrastructure.Identity;

namespace Parus.WebUI.Services
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
            string requestUri = $"{ApiClient.BaseApiUri}/account/register?firstname={firstname}" +
                $"&lastname={lastname}&nickname={nickname}" +
                $"&email={email}&password={password}";

            Console.WriteLine("Registering API call uri: " + requestUri);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri)
            };

            return await httpClient.SendAsync(request);
        }

        public async Task<object> LoginAsync(string nickname, string password)
        {
            string requestUri = $"{ApiClient.BaseApiUri}/account/login?nickname={nickname}&password={password}";
            Console.WriteLine("Login API call uri: " + requestUri);

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri)
            };

            return await httpClient.SendAsync(request);
        }

		public async Task<object> LoginJwtAsync(string nickname, string password)
		{
			string requestUri = $"{ApiClient.BaseApiUri}/account/login?nickname={nickname}&password={password}";
			Console.WriteLine("Login API call uri: " + requestUri);

			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri(requestUri)
			};

			return await httpClient.SendAsync(request);
		}
	}
}
