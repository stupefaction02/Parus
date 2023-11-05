using Microsoft.Extensions.DependencyInjection;

namespace Parus.WebUI.Extensions
{
    public static class ServicesCollectionExtensions
    {
        public static void AddCookieAuthentication(this IServiceCollection services)
        {
            // After this any woulnd able to access any page or method with AuthoeizeAttribute if 
            // it has not cookies configured below
            services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", config =>
            {
                config.Cookie.Name = "AdminCookie";
                config.LoginPath = "/Identity/Authenticate";
            });
        }

    }
}
