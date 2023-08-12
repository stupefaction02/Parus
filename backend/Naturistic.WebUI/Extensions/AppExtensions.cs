using Microsoft.AspNetCore.Builder;
using Naturistic.WebUI.Middlewares;

namespace Naturistic.WebUI.Extensions
{
    public static class AppExtensions
    {
        public static IApplicationBuilder UseDebug(this IApplicationBuilder app)
        {
            app.UseMiddleware<DebugMiddleware>();
            return app;
        }
    }
}
