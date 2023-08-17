
using Cassandra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Naturistic.Backend.Controllers;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Core.Interfaces.Services;
using Naturistic.Infrastructure.DLA.Repositories;
using Naturistic.Infrastructure.Identity;
using Newtonsoft.Json.Linq;
using Xunit;

namespace IdentityTest
{
	public class SimpleTests
	{
		readonly IServiceProvider services =
			Naturistic.Backend.Program.CreateHostBuilder(new string[] { }).Build().Services;

		private T GetService<T>()
		{
			return services.GetRequiredService<T>();
		}

		[Fact]
		public void RequestRecoveryRefAndEditPassword()
		{
			//var repository = GetService<IPasswordRecoveryTokensRepository>();

			//string addUsername = "ivan21";



			//DateTime expireDate = DateTime.Now.AddHours(PasswordRecoveryToken.LifetimeHours);
			//PasswordRecoveryToken recoveryToken = new PasswordRecoveryToken
			//{
			//	Token = token,
			//	User = user,
			//	UserId = user.Id,
			//	ExpireAt = Common.Utils.TimeUtils.ToUnixTimeSeconds(expireDate)
			//};
		}
	}
}