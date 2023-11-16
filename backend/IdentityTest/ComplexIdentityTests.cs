using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Common.Utils;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Parus.Backend.Authentication;
using Parus.Backend.Controllers;
using Parus.Backend.Middlewares;
using Parus.Backend.Services;
using Parus.Core;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Parus.Core.Interfaces.Services;
using Parus.Infrastructure.DLA;
using Parus.Infrastructure.Identity;

namespace IdentityTest
{
    public partial class ComplexIdentityTests
	{
		
		public ComplexIdentityTests()
        {
			Helper.Boot();
		}

		public struct RegistratonContext
		{
			public string username; 
			public string email; 
			public IdentityController controller;
			public IUser user;
        }

		private RegistratonContext RegisterUser()
		{
			string a = Guid.NewGuid().ToString().Substring(0, 8);
            string username = "test_ivan73_" + a;
            string email = "testivan73" + a + "@gmail.com";
            string password = "zx1";

            var controller = Helper.GetIdentityController();

			var um = Helper.GetBackendService<UserManager<ApplicationUser>>();
            IUserRepository users = Helper.GetBackendService<IUserRepository>();
            Thread.Sleep(3000);
            JsonResult registerUser = (JsonResult)
                (controller.Register(username, email, password, Gender.Male, IdentityController.RegisterType.ViewerUser, um, users));

            Assert.True(registerUser.Value != null);

            ApiServerResponse jsonResult;
            using (MemoryStream ms = new MemoryStream())
            {
                JsonSerializer.SerializeAsync(ms, registerUser.Value, typeof(object),
                    new JsonSerializerOptions() { }).GetAwaiter().GetResult();
                var jsonResulltString = Encoding.UTF8.GetString(ms.ToArray());
                jsonResult = JsonSerializer.Deserialize<ApiServerResponse>(jsonResulltString);
            }

            Assert.True(jsonResult != null);
            Assert.True(jsonResult.Success == "Y");

            bool added = Helper.users.Contains(x => x.GetUsername() == username);

            IUser got = Helper.users.One(x => x.GetUsername() == username);

            Assert.True(added);
            Assert.True(a != null);

			return new RegistratonContext { user = got, username = username, email = email, controller = controller };
        }

        [Fact]
		public async void RegisterUser_RequestRecovery_EditPassword_DeleteUser()
		{
            string newpassword = "zx2";

			var ctx = RegisterUser();

            Helper.server.Features.Set<IServerAddressesFeature>(new MyServerAddressesFeature());

			var passwordRecoveryTokens = Helper.GetBackendService<IPasswordRecoveryTokensRepository>();
            var passwordHasher = Helper.GetBackendService<IPasswordHasher<ApplicationUser>>();
            var userManager = Helper.GetBackendService<UserManager<ApplicationUser>>();
            var senderRecoveryEmail = await ctx.controller.SendRecoveryEmail(ctx.username, ctx.email, "ru",
                passwordRecoveryTokens,
                passwordHasher,
                Helper.server, 
                Helper.serverLocalization,
                userManager);

			Assert.IsType<OkResult>(senderRecoveryEmail);

			var repository = Helper.GetBackendService<IPasswordRecoveryTokensRepository>();

			PasswordRecoveryToken tokenAdded = (PasswordRecoveryToken)repository.OneByUser(ctx.user.GetId());
			Assert.True(tokenAdded != null);

			var testCookies = new Dictionary<string, string>();
			testCookies.Add("locale", "ru");
            Helper.editPasswordPage.PageContext = TestContexts.CreatePageContext(testCookies);

			// skip email checking part
			// assumed user got his email with token and went to editpassword page
			var editPasswordModelGet = Helper.editPasswordPage.OnGet(tokenAdded.Token);

			Assert.IsType<PageResult>(editPasswordModelGet);

			var identity = await Helper.CreateIdentityAsync((ApplicationUser)ctx.user, Helper.userManager);
            Helper.editPasswordPage.HttpContext.User = new ClaimsPrincipal(identity);

            Helper.tokens.ClearTracking();
            Helper.users.ClearTracking();
			var editPasswordPageResult = await Helper.editPasswordPage.OnPostAsync(newpassword);

			Assert.IsType<RedirectToPageResult>(editPasswordPageResult);

			ApplicationUser editedUser = (ApplicationUser)Helper.users.One(x => x.GetUsername() == ctx.username);
			Assert.NotNull(editedUser);

			bool passwordChanged = Helper.passwordHasher.VerifyHashedPassword(editedUser, editedUser.PasswordHash, newpassword) == PasswordVerificationResult.Success;
			Assert.True(passwordChanged);

			//tokens.Delete();

			PasswordRecoveryToken deletedToken = (PasswordRecoveryToken)repository.OneByUser(editedUser.GetId());
			Assert.Null(deletedToken);

			DeleteUser(ctx.username);
		}

        private void DeleteUser(string username)
        {
            Helper.users.ClearTracking();
            Helper.users.RemoveOne(username: username);

            bool stillExists = Helper.users.Contains(x => x.GetUsername() == username);

            // second check 
            IUser deletedUser = Helper.users.One(x => x.GetUsername() == username);

            Assert.False(stillExists);
            Assert.Null(deletedUser);
        }

        [Fact]
		public async void RegisterUser_DeleteUser()
		{
            RegistratonContext ctx = RegisterUser();

            DeleteUser(ctx.username);
		}

		[Fact]
		public async void RegisterUser_LoginUSer_ConfirmUser_DeleteUser()
		{
			RegistratonContext ctx = RegisterUser();

            var codes = Helper.GetBackendService<IConfrimCodesRepository>();
            var emails = Helper.GetBackendService<IEmailService>();
            var um = Helper.GetBackendService<UserManager<ApplicationUser>>();
            Thread.Sleep(3000);
            JsonResult createCodeResult = 
				(JsonResult)(await ctx.controller.CreateVerificationCodeAsync(ctx.username, true, 
                codes, 
                emails,
                um));

			ApiServerResponse createCodeResultJsonResult;
			using (MemoryStream ms = new MemoryStream())
			{
				await JsonSerializer.SerializeAsync(ms, createCodeResult.Value, typeof(object),
					new JsonSerializerOptions() { });
				var r1 = Encoding.UTF8.GetString(ms.ToArray());
				createCodeResultJsonResult = JsonSerializer.Deserialize<ApiServerResponse>(r1);
			}

			Assert.True(createCodeResultJsonResult != null);
			Assert.True(createCodeResultJsonResult.Success == "Y");

			bool codeAdded = Helper.confirmCodes.Contains(ctx.user.GetId());
			Assert.True(codeAdded);

			ConfirmCode addedCode = (ConfirmCode)Helper.confirmCodes.OneByUser(ctx.user.GetId());
			Assert.NotNull(addedCode);

			Helper.users.ClearTracking();

            IUserRepository users = Helper.GetBackendService<IUserRepository>();
            JsonResult verifyResult =
				(JsonResult)(await ctx.controller.VerifyAccountAsync(ctx.username, addedCode.Code, users, codes));

			ApiServerResponse verifyResultResultJsonResult;
			using (MemoryStream ms = new MemoryStream())
			{
				await JsonSerializer.SerializeAsync(ms, verifyResult.Value, typeof(object),
					new JsonSerializerOptions() { });
				var r2 = Encoding.UTF8.GetString(ms.ToArray());
				verifyResultResultJsonResult = JsonSerializer.Deserialize<ApiServerResponse>(r2);
			}

			Assert.True(verifyResultResultJsonResult != null);
			Assert.True(verifyResultResultJsonResult.Success == "Y");

			ApplicationUser sameUser = (ApplicationUser)Helper.users.One(x => x.GetUsername() == ctx.username);

			Assert.True(sameUser.EmailConfirmed);

			bool codeStillExists = Helper.confirmCodes.Contains(ctx.user.GetId());
			Assert.False(codeStillExists);

			ConfirmCode stillAddedCode = (ConfirmCode)Helper.confirmCodes.OneByUser(ctx.user.GetId());
			Assert.Null(stillAddedCode);

			DeleteUser(ctx.username);
		}


        //[Fact]
        public async void GetHostUser_CreateBroadcast_DeleteBroadcast()
		{
            BroadcastController bc = new Parus.Backend.Controllers.BroadcastController();

            UserManager<ApplicationUser> um = Helper.GetBackendService<UserManager<ApplicationUser>>();
            ApplicationDbContext context = Helper.GetBackendService<ApplicationDbContext>();
            ApplicationIdentityDbContext ic = Helper.GetBackendService<ApplicationIdentityDbContext>();
            IBroadcastInfoRepository br = Helper.GetBackendService<IBroadcastInfoRepository>();
            BroadcastControl bco = Helper.GetBackendService<BroadcastControl>();

			string hostUserName = "test_ivan122";
            IUser hostUser = Helper.users.One(x => x.GetUsername() == hostUserName);
            string hostUserId = hostUser.GetId();

            Assert.True(hostUser != null);

            ClaimsIdentity identity = await Helper.CreateIdentityAsync((ApplicationUser)hostUser, um);

            bc.User = new ClaimsPrincipal(identity);

            IActionResult result = await bc.StartBroadcast("title#1", 1, new int[] { 1, 2 }, context, ic, bco);

			Assert.IsType<OkResult>(result);

            BroadcastInfo createdBroadcast = br.OneLazy(x => x.HostUserId == hostUserId);

			Assert.NotNull(createdBroadcast);
			Assert.True(createdBroadcast.HostUserId == hostUserId);

            IActionResult stopBroadcastResult = await bc.StopBroadcast("", br, ic, bco);

            Assert.IsType<OkResult>(result);

            BroadcastInfo alreadyDeleted = br.OneLazy(x => x.HostUserId == hostUserId);

            Assert.Null(alreadyDeleted);
        }


		private string fingerPrint;
		private string resreshToken;
        [Fact]
        public async void FailToLoginAndGetRefreshToken()
		{
			IdentityController identityController = Helper.GetIdentityController();
            ApplicationIdentityDbContext context = Helper.GetBackendService<ApplicationIdentityDbContext>();
            CheckingLoggingInMiddleware clmw = new CheckingLoggingInMiddleware(x => Next(x));

            /* Registration Starts */

            RegistratonContext ctx = RegisterUser();

            ApplicationUser newUser = (ApplicationUser)Helper.users.One(x => x.GetUsername() == ctx.username);

			Assert.NotNull(newUser);

            List<Claim> claims = new List<Claim>
            {
				new Claim(ClaimsIdentity.DefaultNameClaimType, ctx.username)
			};

            JwtSecurityToken jwt = new JwtSecurityToken(
                    issuer: JwtAuthOptions.ISSUER,
                    audience: JwtAuthOptions.AUDIENCE,
                    notBefore: DateTime.UtcNow,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromSeconds(1)),
                    signingCredentials: new SigningCredentials(JwtAuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

			string strJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var testCookies = new Dictionary<string, string>
            {
                { "locale", "ru" },
                { "JWT", strJwt }
            };

            var testHeader = new Dictionary<string, string>
            {
                { "Authorization", "Bearer " + strJwt }
            };

            fingerPrint = Guid.NewGuid().ToString();
            resreshToken = Guid.NewGuid().ToString().Replace("-", "");

			int expTs = DateTimeUtils.ToUnixTimeSeconds(
				DateTime.UtcNow.Add(RefreshSession.LifeTime) );
            RefreshSession newRs = new RefreshSession
            {
                // TODO: Replace with something more efficeint
                Token = resreshToken,
                Fingerprint = fingerPrint,
                ExpiresAt = expTs,
				User = newUser,
                
            }; 

			context.RefreshSessions.Add(newRs);
			context.SaveChanges();

			/* Registration Ends */

            Thread.Sleep(1000);

            HttpContext httpContext = TestContexts.CreateContext1(testCookies, testHeader);
            httpContext.RequestServices = Helper.backendServices;
            TestIdentity te = new TestIdentity();
			te._IsAuthenticated = false;
            httpContext.User = new ClaimsPrincipal(te);
			Console.WriteLine(resreshToken);
            await clmw.Invoke(httpContext);

			async Task Next(HttpContext ctx)
			{
				Assert.False(ctx.User.Identity.IsAuthenticated);

                ((TestHttpRequest)(ctx.Request)).AddCookie("refreshToken", resreshToken);

                identityController.ControllerContext.HttpContext = ctx;

				var res = identityController.RefreshToken(fingerPrint, context).GetAwaiter().GetResult();

				Assert.IsNotType<UnauthorizedResult>(res);
				Assert.IsNotType<ForbidResult>(res);

                return;
			};

			var added = context.RefreshSessions.SingleOrDefault(x => x.UserId == newUser.Id);
			
			Assert.NotNull(added);

            var previousSession = context.RefreshSessions.SingleOrDefault(x => x.Token == resreshToken);

			Assert.Null(previousSession);
            /* cleaning */

            context.RefreshSessions.Remove(added);
            Helper.users.RemoveOne(newUser.UserName);

			context.SaveChanges();
        }
    }
}