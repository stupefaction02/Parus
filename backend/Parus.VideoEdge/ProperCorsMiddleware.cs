using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Parus.VideoEdge
{
    public class ProperCorsMiddleware
    {
        private readonly RequestDelegate next;

        public ProperCorsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            return next(context);
        }
    }
}
