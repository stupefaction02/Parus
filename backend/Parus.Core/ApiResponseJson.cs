using System.Text.Json.Serialization;

namespace Parus.Core
{
    public class ApiResponseJson1
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("success")]
        public string Success { get; set; }
    }
}