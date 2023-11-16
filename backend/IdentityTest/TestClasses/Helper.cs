using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Parus.Core.Interfaces;
using Parus.Core.Interfaces.Repositories;
using Parus.Core.Interfaces.Services;
using Parus.Infrastructure.Identity;
using Parus.WebUI.Pages.Identity;

namespace IdentityTest
{
	public static class Helper
	{
        static public ILocalizationService serverLocalization;
        static public IServer server;
        static public ILocalizationService webLocalization;
        static public IPasswordRecoveryTokensRepository tokens;
        static public Logger<Parus.WebUI.Pages.Identity.EditPasswordModel> weblogger;
        static public IUserRepository users;
        static public UserManager<ApplicationUser> userManager;
        static public IPasswordHasher<ApplicationUser> passwordHasher;
        static public EditPasswordModel editPasswordPage;
        static public IConfrimCodesRepository confirmCodes;

        static public IServiceProvider backendServices;
        static public IServiceProvider webUiServices;

        private static IHost backend;
        private static IHost webui;

        public static T GetBackendService<T>()
        {
            return backend.Services.GetRequiredService<T>();
        }

        public static T GetWebUIService<T>()
        {
            return webui.Services.GetRequiredService<T>();
        }

        public static Parus.Backend.Controllers.IdentityController GetIdentityController()
        {
            var a12 = GetBackendService<ILogger<Parus.Backend.Controllers.IdentityController>>();

            return new Parus.Backend.Controllers.IdentityController(a12);
        }

        public static void Boot()
		{
            backend = Parus.Backend.Program.CreateHostBuilder(new string[] { }).Build();
            webui = Parus.WebUI.Program.CreateHostBuilder(new string[] { }).Build();

            backendServices = backend.Services;

            serverLocalization = Helper.GetBackendService<ILocalizationService>();
            server = Helper.GetBackendService<IServer>();
            webLocalization = Helper.GetWebUIService<ILocalizationService>();
            tokens = Helper.GetWebUIService<IPasswordRecoveryTokensRepository>();
            Logger<EditPasswordModel> weblogger = null;
            users = Helper.GetBackendService<IUserRepository>();
            userManager = Helper.GetWebUIService<UserManager<ApplicationUser>>();
            passwordHasher = Helper.GetWebUIService<IPasswordHasher<ApplicationUser>>();
            confirmCodes = Helper.GetBackendService<IConfrimCodesRepository>();

            editPasswordPage = new EditPasswordModel
                (users, webLocalization, tokens, userManager, passwordHasher, weblogger);
        }

        public static async Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, user.GetUsername())
                };

            return new TestIdentity(claims);
        }
    }
}