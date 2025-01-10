using Microsoft.AspNetCore.Builder;
using Parus.Infrastructure.Middlewares;

namespace Parus.WebUI.Extensions
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
