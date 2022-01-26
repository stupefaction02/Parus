using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.IO;
using System.Net.Http;

using Newtonsoft.Json;

namespace Naturistic.Core.Extensions
{
    public static class HttpClientExtensions
    {
        private static readonly JsonSerializer jsonSerializer = CreateSerializer();

        private static JsonSerializer CreateSerializer()
            => new JsonSerializer(); //{ Converters = { new StoreElementJsonConverter() } };

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            using (var stream = await content.ReadAsStreamAsync())
            {
                var jsonReader = new JsonTextReader(new StreamReader(stream));

                return jsonSerializer.Deserialize<T>(jsonReader);
            }
        }

        public static async Task<HttpResponseMessage> PutJsonAsync<T>(this HttpClient client, string url, T value)
        {
            return await SendJsonAsync<T>(client, HttpMethod.Put, url, value);
        }

        public static async Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, string url, T value)
        {
            return await SendJsonAsync<T>(client, HttpMethod.Post, url, value);
        }

        public static Task<HttpResponseMessage> SendJsonAsync<T>(this HttpClient client, HttpMethod method, string url, T value)
        {
            var stream = new MemoryStream();
            var writer = new JsonTextWriter(new StreamWriter(stream));

            jsonSerializer.Serialize(writer, value);

            writer.Flush();

            stream.Position = 0;

            var request = new HttpRequestMessage(method, url)
            {
                Content = new StreamContent(stream)
            };

            request.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");

            return client.SendAsync(request);
        }
    }
}
