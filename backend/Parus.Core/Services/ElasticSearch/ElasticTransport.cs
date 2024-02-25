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

        HttpClient client;

        Uri uri;
        public ElasticTransport(string uri, (string, string) auth)
        {
            var handler = new HttpClientHandler();

            // basically force program to trust the certificate that elastic sent
           // handler.ServerCertificateCustomValidationCallback +=
         //       (sender, cert, chain, sslPolicyErrors) => true;

            this.client = new HttpClient(handler);

            this.uri = new Uri(uri);

            this.client.BaseAddress = this.uri;

            this.client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(auth.Item1, auth.Item2);


            //this.client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        }

        public void Connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            socket.Connect(IPAddress.Loopback, 9200);

            socket.Blocking = true;
        }

        public async Task<(Stream, HttpStatusCode)> GetContentAsync(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {

                Method = HttpMethod.Get,
                RequestUri = new Uri(client.BaseAddress + uri)
            };

            HttpResponseMessage responseMessage = await client.SendAsync(request);

            return (await responseMessage.Content.ReadAsStreamAsync(), responseMessage.StatusCode);
        }

        public async Task<HttpStatusCode> DeleteAsync(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri(client.BaseAddress + uri)
            };

            HttpResponseMessage responseMessage = await client.SendAsync(request);

            return responseMessage.StatusCode;
        }

        public async Task<HttpStatusCode> PutStringAsync(string uri, string data)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(client.BaseAddress + uri),
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage responseMessage = await client.SendAsync(request);

            return responseMessage.StatusCode;
        }

        public async Task<HttpStatusCode> PostString(string uri, string data)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(client.BaseAddress + uri),
                Content = new StringContent(data, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage responseMessage = await client.SendAsync(request);

            return responseMessage.StatusCode;
        }

        public async Task<HttpStatusCode> Header(string uri)
        {
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri(client.BaseAddress + uri)
            };

            HttpResponseMessage responseMessage = await client.SendAsync(request);

            return responseMessage.StatusCode;
        }
    }
}
