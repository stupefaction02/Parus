using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Parus.Core.Services.ElasticSearch
{
    public class ElasticTransport
    {
        static private Socket socket;

        HttpClient _client;

        Uri host;
        public ElasticTransport(string host, (string, string) auth)
        {
            HttpClientHandler handler = new HttpClientHandler();

            _client = new HttpClient(handler);

            //_client.Timeout = new TimeSpan(0, 0, 5);

            this.host = new Uri(host);

            _client.BaseAddress = this.host;

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(auth.Item1, auth.Item2);

            //this.client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        }

        public async Task<(Stream, HttpStatusCode)> GetContentAsync(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                
                Method = HttpMethod.Get,
                RequestUri = new Uri(_client.BaseAddress + uri)
            };

            HttpResponseMessage responseMessage = await _client.SendAsync(request);

            return (await responseMessage.Content.ReadAsStreamAsync(), responseMessage.StatusCode);
        }

        public async Task<HttpStatusCode> DeleteAsync(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_client.BaseAddress + uri)
            };

            HttpResponseMessage responseMessage = await _client.SendAsync(request);

            return responseMessage.StatusCode;
        }

        public async Task<HttpStatusCode> PutStringAsync(string uri, string data)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(_client.BaseAddress + uri),
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage responseMessage = await _client.SendAsync(request);

            return responseMessage.StatusCode;
        }

        public async Task<(HttpStatusCode, string)> GetStringAsync(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(_client.BaseAddress + uri)
            };

            HttpResponseMessage responseMessage = await _client.SendAsync(request);

            return (responseMessage.StatusCode, await responseMessage.Content.ReadAsStringAsync());
        }

        public async Task<HttpStatusCode> PostStringAsync(string uri, string data)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress + uri),
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage responseMessage = await _client.SendAsync(request);

            return responseMessage.StatusCode;
        }

        public async Task<HttpStatusCode> PostString(string uri, string data)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_client.BaseAddress + uri),
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage responseMessage = await _client.SendAsync(request);

            return responseMessage.StatusCode;
        }

        /// <summary>
        /// Sends with "/" path
        /// </summary>
        public HttpStatusCode Header()
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Head,
                RequestUri = new Uri(_client.BaseAddress.ToString())
            };

            HttpResponseMessage responseMessage = _client.SendAsync(request).GetAwaiter().GetResult();

            return responseMessage.StatusCode;
        }
    }
}
