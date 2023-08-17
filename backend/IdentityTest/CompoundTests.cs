
using System.Text;
using System.Text.Json;
using Cassandra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Naturistic.Backend.Controllers;
using Naturistic.Common;
using Naturistic.Core;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Core.Interfaces.Services;
using Naturistic.Infrastructure.DLA.Repositories;
using Naturistic.Infrastructure.Identity;
using Org.BouncyCastle.Asn1.Sec;
using Xunit;

namespace IdentityTest
{
	public class CompoundTests
	{
		readonly IServiceProvider services =
			Naturistic.Backend.Program.CreateHostBuilder(new string[] { }).Build().Services;

		private T GetService<T>()
		{
			return services.GetRequiredService<T>();
		}

		private IdentityController GetIdentityController()
		{
			var a = GetService<IWebHostEnvironment>();
			var a1 = GetService<UserManager<ApplicationUser>>();
			var a2 = GetService<SignInManager<ApplicationUser>>();
			var a3 = GetService<IConfiguration>();
			var a4 = GetService<IViewerUsersRepository>();
			var a5 = GetService<IChatsRepository>();
			var a6 = GetService<IUserRepository>();
			var a7 = GetService<IConfrimCodesRepository>();
			var a9 = GetService<IPasswordRecoveryTokensRepository>();
			var a10 = GetService<IEmailService>();
			var a11 = GetService<IPasswordHasher<ApplicationUser>>();
			var a12 = GetService<ILogger<IdentityController>>();

			return new IdentityController(a, a1, a2, a3, a4, a5, a6, a7, a9, a10, a11, a12);
		}

		
		public async void RequestRecoveryRefAndEditPassword()
		{
			ILocalizationService localization = GetService<ILocalizationService>();
			IServer server = GetService<IServer>();

			string username = "test_ivan21";
			string email = "testivan21@gcom";
			string password = "zx1";
			
			var controller = GetIdentityController();

			JsonResult registerUser = (JsonResult)await controller.Register(username, email, password, Gender.Male, IdentityController.RegisterType.ViewerUser);
			var senderRecoveryEmail = await controller.SendRecoveryEmail(username, email, "ru", server, localization);

			//var registerUserResponse = JsonSerializer.Deserialize<ApiServerResponse>();

			//Assert.True(registerUserResponse.Success);
			Assert.IsType<OkResult>(senderRecoveryEmail);

			var repository = GetService<IPasswordRecoveryTokensRepository>();

			var added = repository.GetTokenByUsername(username);

			Assert.True(added != null);
		}

		public void PasswordRecoveryTokensRepository_ContainsUser()
		{
			


		}

		[Fact]
		public async void Register_User()
		{
			IUserRepository users = GetService<IUserRepository>();

			string username = "test_ivan27";
			string email = "testivan27@gcom";
			string password = "zx1";

			var controller = GetIdentityController();

			JsonResult registerUser = (JsonResult)await controller.Register(username, email, password, Gender.Male, IdentityController.RegisterType.ViewerUser);

			Assert.True(registerUser.Value != null);

			ApiServerResponse jsonResult;
			using (MemoryStream ms = new MemoryStream())
			{
				await JsonSerializer.SerializeAsync(ms, registerUser.Value, typeof(object), new JsonSerializerOptions(JsonSerializerDefaults.Web));
				var jsonResulltString = Encoding.ASCII.GetString(ms.ToArray());
				jsonResult = JsonSerializer.Deserialize<ApiServerResponse>(jsonResulltString);
			}

			Assert.True(jsonResult != null);
			Assert.True(jsonResult.Success == "Y");

			bool added = await users.ContainsAsync(x => x.GetUsername() == username);

			Assert.True(added);

			bool deleted = await users.DeleteAsync(username: username);

			Assert.True(deleted);
		}
	}
}