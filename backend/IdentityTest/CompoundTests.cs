
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Naturistic.Core;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Core.Interfaces.Services;
using Naturistic.Infrastructure.Identity;
using Naturistic.WebUI.Pages.Identity;

namespace IdentityTest
{
	public partial class CompoundTests
	{
		IServiceProvider services =
			Naturistic.Backend.Program.CreateHostBuilder(new string[] { }).Build().Services;

		IServiceProvider services1 =
			Naturistic.WebUI.Program.CreateHostBuilder(new string[] { }).Build().Services;

		private T GetBackendService<T>()
		{
			return services.GetRequiredService<T>();
		}

		private T GetWebUIService<T>()
		{
			return services1.GetRequiredService<T>();
		}

		private Naturistic.Backend.Controllers.IdentityController GetIdentityController()
		{
			var a = GetBackendService<IWebHostEnvironment>();
			var a1 = GetBackendService<UserManager<ApplicationUser>>();
			var a2 = GetBackendService<SignInManager<ApplicationUser>>();
			var a3 = GetBackendService<IConfiguration>();
			var a4 = GetBackendService<IViewerUsersRepository>();
			var a5 = GetBackendService<IChatsRepository>();
			var a6 = GetBackendService<IUserRepository>();
			var a7 = GetBackendService<IConfrimCodesRepository>();
			var a9 = GetBackendService<IPasswordRecoveryTokensRepository>();
			var a10 = GetBackendService<IEmailService>();
			var a11 = GetBackendService<IPasswordHasher<ApplicationUser>>();
			var a12 = GetBackendService<ILogger<Naturistic.Backend.Controllers.IdentityController>>();

			return new Naturistic.Backend.Controllers.IdentityController(a, a1, a2, a3, a4, a5, a6, a7, a9, a10, a11, a12);
		}

		public class MyServerAddressesFeature : IServerAddressesFeature
		{
			public ICollection<string> Addresses { get; } = new string[2]
			{
				"", ""
			};

			public bool PreferHostingUrls { get; set; }
		}

		ILocalizationService serverLocalization;
		IServer server;
		ILocalizationService webLocalization;
		IPasswordRecoveryTokensRepository tokens;
		Logger<Naturistic.WebUI.Pages.Identity.EditPasswordModel> weblogger;
		IUserRepository users;
		UserManager<ApplicationUser> userManager;
		IPasswordHasher<ApplicationUser> passwordHasher;
		EditPasswordModel editPasswordPage;
		IConfrimCodesRepository confirmCodes;
		public CompoundTests()
        {
			var b1 = Naturistic.Backend.Program.CreateHostBuilder(new string[] { }).Build();
			var b2 = Naturistic.WebUI.Program.CreateHostBuilder(new string[] { }).Build();

			//Task.Run(async () => 
			//{
			//	b1.RunAsync();
			//	b2.RunAsync();
			//});

			services = b1.Services;
			services1 = b2.Services;

			serverLocalization = GetBackendService<ILocalizationService>();
			server = GetBackendService<IServer>();
			webLocalization = GetWebUIService<ILocalizationService>();
			tokens = GetWebUIService<IPasswordRecoveryTokensRepository>();
			Logger<EditPasswordModel> weblogger = null;
			users = GetBackendService<IUserRepository>();
			userManager = GetWebUIService<UserManager<ApplicationUser>>();
			passwordHasher = GetWebUIService<IPasswordHasher<ApplicationUser>>();
			confirmCodes = GetBackendService<IConfrimCodesRepository>();



			editPasswordPage = new EditPasswordModel
				(users, webLocalization, tokens, userManager, passwordHasher, weblogger);
		}

        [Fact]
		public async void RegisterUser_RequestRecovery_EditPassword_DeleteUser()
		{
			string username = "test_ivan73";
			string email = "testivan73@gmail.com";
			string password = "zx1";
			string newpassword = "zx2";

			var controller = GetIdentityController();

			JsonResult registerUser = (JsonResult)await controller.Register(username, email, password, Gender.Male, Naturistic.Backend.Controllers.IdentityController.RegisterType.ViewerUser);

			Assert.True(registerUser.Value != null);

			ApiServerResponse jsonResult;
			using (MemoryStream ms = new MemoryStream())
			{
				await JsonSerializer.SerializeAsync(ms, registerUser.Value, typeof(object),
					new JsonSerializerOptions() { });
				var jsonResulltString = Encoding.UTF8.GetString(ms.ToArray());
				jsonResult = JsonSerializer.Deserialize<ApiServerResponse>(jsonResulltString);
			}

			Assert.True(jsonResult != null);
			Assert.True(jsonResult.Success == "Y");

			bool added = users.Contains(x => x.GetUsername() == username);

			IUser a = users.One(x => x.GetUsername() == username);

			Assert.True(added);
			Assert.True(a != null);

			server.Features.Set<IServerAddressesFeature>(new MyServerAddressesFeature());

			var senderRecoveryEmail = await controller.SendRecoveryEmail(username, email, "ru", server, serverLocalization);

			Assert.IsType<OkResult>(senderRecoveryEmail);

			var repository = GetBackendService<IPasswordRecoveryTokensRepository>();

			PasswordRecoveryToken tokenAdded = (PasswordRecoveryToken)repository.OneByUser(a.GetId());
			Assert.True(tokenAdded != null);

			var testCookies = new Dictionary<string, string>();
			testCookies.Add("locale", "ru");
			editPasswordPage.PageContext = TestContexts.CreateHttpContext(testCookies);

			// skip email checking part
			// assumed user got his email with token and went to editpassword page
			var editPasswordModelGet = editPasswordPage.OnGet(tokenAdded.Token);

			Assert.IsType<PageResult>(editPasswordModelGet);

			var identity = await CreateIdentityAsync((ApplicationUser)a, userManager);
			editPasswordPage.HttpContext.User = new ClaimsPrincipal(identity);

			tokens.ClearTracking();
			users.ClearTracking();
			var editPasswordPageResult = await editPasswordPage.OnPostAsync(newpassword);

			Assert.IsType<RedirectToPageResult>(editPasswordPageResult);

			ApplicationUser editedUser = (ApplicationUser)users.One(x => x.GetUsername() == username);
			Assert.NotNull(editedUser);

			bool passwordChanged = passwordHasher.VerifyHashedPassword(editedUser, editedUser.PasswordHash, newpassword) == PasswordVerificationResult.Success;
			Assert.True(passwordChanged);

			//tokens.Delete();

			PasswordRecoveryToken deletedToken = (PasswordRecoveryToken)repository.OneByUser(editedUser.GetId());
			Assert.Null(deletedToken);

			users.ClearTracking();
			users.RemoveOne(username: username);

			bool stillExists = users.Contains(x => x.GetUsername() == username);

			// second check 
			IUser deletedUser = users.One(x => x.GetUsername() == username);

			Assert.False(stillExists);
			Assert.Null(deletedUser);
		}

		private async Task<ClaimsIdentity> CreateIdentityAsync(ApplicationUser user, UserManager<ApplicationUser> userManager)
		{
			List<Claim> claims = new List<Claim>
				{
					new Claim(ClaimsIdentity.DefaultNameClaimType, user.GetUsername())
				};

			return new ClaimsIdentity(claims);
		}

		[Fact]
		public async void RegisterUser_DeleteUser()
		{
			IUserRepository users = GetBackendService<IUserRepository>();

			string username = "test_ivan101";
			string email = "testivan101@gcom";
			string password = "zx1";

			var controller = GetIdentityController();

			JsonResult registerUser = (JsonResult)await controller.Register(username, email, password, Gender.Male, Naturistic.Backend.Controllers.IdentityController.RegisterType.ViewerUser);

			Assert.True(registerUser.Value != null);

			ApiServerResponse jsonResult;
			using (MemoryStream ms = new MemoryStream())
			{
				await JsonSerializer.SerializeAsync(ms, registerUser.Value, typeof(object), 
					new JsonSerializerOptions() {  } );
				var jsonResulltString = Encoding.UTF8.GetString(ms.ToArray());
				jsonResult = JsonSerializer.Deserialize<ApiServerResponse>(jsonResulltString);
			}

			Assert.True(jsonResult != null);
			Assert.True(jsonResult.Success == "Y");

			bool added = users.Contains(x => x.GetUsername() == username);

			IUser a = users.One(x => x.GetUsername() == username);

			Assert.True(added);
			Assert.True(a != null);

			users.RemoveOne(username: username);

			bool stillExists = users.Contains(x => x.GetUsername() == username);

			// second check 
			IUser b = users.One(x => x.GetUsername() == username);

			Assert.False(stillExists);
			Assert.True(b == null);
		}

		[Fact]
		public async void RegisterUser_LoginUSer_ConfirmUser_DeleteUser()
		{
			#region Register
			IUserRepository users = GetBackendService<IUserRepository>();

			string username = "test_ivan124";
			string email = "testivan124@gcom";
			string password = "zx1";

			var controller = GetIdentityController();

			JsonResult registerUser = (JsonResult)await controller.Register(username, email, password, Gender.Male, Naturistic.Backend.Controllers.IdentityController.RegisterType.ViewerUser);

			Assert.True(registerUser.Value != null);

			ApiServerResponse jsonResult;
			using (MemoryStream ms = new MemoryStream())
			{
				await JsonSerializer.SerializeAsync(ms, registerUser.Value, typeof(object),
					new JsonSerializerOptions() { });
				var jsonResulltString = Encoding.UTF8.GetString(ms.ToArray());
				jsonResult = JsonSerializer.Deserialize<ApiServerResponse>(jsonResulltString);
			}

			Assert.True(jsonResult != null);
			Assert.True(jsonResult.Success == "Y");

			bool added = users.Contains(x => x.GetUsername() == username);

			IUser a = users.One(x => x.GetUsername() == username);

			Assert.True(added);
			Assert.True(a != null);

			#endregion

			JsonResult createCodeResult = 
				(JsonResult)(await controller.CreateVerificationCodeAsync(username));

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

			bool codeAdded = confirmCodes.Contains(a.GetId());
			Assert.True(codeAdded);

			ConfirmCode addedCode = (ConfirmCode)confirmCodes.OneByUser(a.GetId());
			Assert.NotNull(addedCode);

			users.ClearTracking();
			JsonResult verifyResult =
				(JsonResult)(await controller.VerifyAccountAsync(username, addedCode.Code));

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

			ApplicationUser sameUser = (ApplicationUser)users.One(x => x.GetUsername() == username);

			Assert.True(sameUser.EmailConfirmed);

			bool codeStillExists = confirmCodes.Contains(a.GetId());
			Assert.False(codeStillExists);

			ConfirmCode stillAddedCode = (ConfirmCode)confirmCodes.OneByUser(a.GetId());
			Assert.Null(stillAddedCode);
		}
	}
}