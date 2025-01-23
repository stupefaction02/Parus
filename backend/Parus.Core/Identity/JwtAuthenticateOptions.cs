using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Parus.Core.Identity
{
    public class RefreshTokenOptions
    {
        public int LifetimeHours { get; set; }
    }

    public class JwtAuthOptions
    {
        public string SecretKey { get; set; } = "<!{_Secr-<>-etKey!{_>";

        public string Issuer { get; set; }

        public string Audience { get; set; }

        public int LifetimeMinutes { get; set; }
    }

    // TODO: Set up from configs
    public class JwtAuthOptions2
    {
        public const string ISSUER = "https://localhost:5002";
        public const string AUDIENCE = "https://localhost:5002";
        const string KEY = "{amogus!1000!}{zzyzzyy}1234567890!ilovejwttokenssomuchitsunreal!forreal1290!}}}!asdewegwg!!!12!!!!}{}{}";
        // in minutes
        public const int LIFETIME = 60 * 24 * 3 * 7 * 31;

        public static TimeSpan Lifetime;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.Default.GetBytes(KEY));
        }
    }
}
