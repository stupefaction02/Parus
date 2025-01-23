using Microsoft.AspNetCore.Http;

namespace Parus.WebUI.Middlewares
{
    public class DummyRestrictionRuleBase
    {

        public bool Check(HttpContext httpContext)
        {
            var rip = httpContext.Connection.RemoteIpAddress;

            return false;
        }
    }
}