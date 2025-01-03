using Parus.Core.Entities;
using System.Text.Json.Serialization;

namespace Parus.Backend.Controllers
{
    public partial class IdentityController
    {
        public class ParusUserRegistrationJsonDTO
        {
            [JsonPropertyName("username")]
            public string Username { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("password")]
            public string Password { get; set; }

            [JsonPropertyName("gender")]
            public Gender Gender { get; set; }
        }
    }
}
