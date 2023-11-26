using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Parus.Backend.Controllers;
using Parus.Core;
using Parus.Core.Entities;
using Parus.Core.Interfaces;
using Parus.Core.Interfaces.Repositories;
using Parus.Core.Interfaces.Services;
using Parus.Infrastructure.Identity;
using Parus.WebUI.Pages.Identity;

namespace IdentityTest
{
    public struct RegistratonContext
    {
        public string password;
        public string username;
        public string email;
        public IdentityController controller;
        public IUser user;

        public RefreshSession RefreshSession { get; internal set; }
    }

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

        static bool booted;

        public static void Boot()
		{
            if (booted) return;

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

            booted = true;
        }

        public static void DeleteUser(string username)
        {
            Helper.users.ClearTracking();

            string id = Helper.users.One(x => x.GetUsername() == username).GetId();

            ApplicationIdentityDbContext idc = Helper.GetBackendService<ApplicationIdentityDbContext>();
            var notDeletedRefreshSession = idc.RefreshSessions.SingleOrDefault(x => x.UserId == id);

            idc.RefreshSessions.Remove(notDeletedRefreshSession);

            idc.SaveChanges();

            var deletedRefreshSession = idc.RefreshSessions.SingleOrDefault(x => x.UserId == id);

            Assert.Null(deletedRefreshSession);

            Helper.users.RemoveOne(username: username);

            bool stillExists = Helper.users.Contains(x => x.GetUsername() == username);

            // second check 
            IUser deletedUser = Helper.users.One(x => x.GetUsername() == username);

            Assert.False(stillExists);
            Assert.Null(deletedUser);
        }

        public static RegistratonContext RegisterUser(bool addIdentity = false)
        {
            string a = Guid.NewGuid().ToString().Substring(0, 8);
            string username = "test_ivan73_" + a;
            string email = "testivan73" + a + "@gmail.com";
            string password = "zx1";

            IdentityController controller = Helper.GetIdentityController();

            var um = Helper.GetBackendService<UserManager<ApplicationUser>>();
            IUserRepository users = Helper.GetBackendService<IUserRepository>();
            ApplicationIdentityDbContext idc = Helper.GetBackendService<ApplicationIdentityDbContext>();

            Thread.Sleep(5000);
            JsonResult registerUser = (JsonResult)
                (controller.Register(username, email, password, Gender.Male, IdentityController.RegisterType.ViewerUser, um, users, idc));

            Assert.True(registerUser.Value != null);

            AuthenticateResponseJson jsonResult;
            using (MemoryStream ms = new MemoryStream())
            {
                JsonSerializer.SerializeAsync(ms, registerUser.Value, typeof(object),
                    new JsonSerializerOptions() { }).GetAwaiter().GetResult();
                var jsonResulltString = Encoding.UTF8.GetString(ms.ToArray());
                jsonResult = JsonSerializer.Deserialize<AuthenticateResponseJson>(jsonResulltString);
            }

            Console.WriteLine(jsonResult.RefreshToken.Token);
            Assert.True(jsonResult != null);
            Assert.True(jsonResult.Success);

            bool added = Helper.users.Contains(x => x.GetUsername() == username);

            IUser got = Helper.users.One(x => x.GetUsername() == username);

            Assert.True(added);
            Assert.True(a != null);

            RefreshSession addedRefreshToken = idc.RefreshSessions.SingleOrDefault(x => x.UserId == got.GetId());

            Assert.NotNull(addedRefreshToken);

            var ctx = new RegistratonContext { RefreshSession = addedRefreshToken, user = got, username = username, email = email, controller = controller, password = password };

            if (addIdentity)
            {
                ClaimsIdentity identity = 
                    Helper.CreateIdentityAsync((ApplicationUser)got, um)
                    .GetAwaiter()
                    .GetResult();

                controller.User = new ClaimsPrincipal(identity);
            }

            return ctx;
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