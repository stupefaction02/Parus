
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Parus.Backend.Controllers;
using Parus.Core;
using Parus.Core.Entities;
using Parus.Core.Interfaces;
using Parus.Core.Interfaces.Repositories;
using Parus.Core.Interfaces.Services;
using Parus.Infrastructure.DLA.Repositories;
using Parus.Infrastructure.Identity;
using Newtonsoft.Json.Linq;
using Xunit;
using System.Formats.Asn1;

namespace IdentityTest
{
	public class PasswordsTests
    {
        public PasswordsTests()
        {
            Helper.Boot();
        }

        [Fact]
        public void CheckPassword_Success()
        {
            var ctx = Helper.RegisterUser(addIdentity: true);

            var sm = Helper.GetBackendService<SignInManager<ApplicationUser>>();
            var users = Helper.GetBackendService<IUserRepository>();
            JsonResult checkPasswordResult = 
                (JsonResult)
                (ctx.controller.CheckPassword(ctx.password, sm, users)
                .GetAwaiter()
                .GetResult());

            ApiResponseJson response = 
                DeserializeResponse<ApiResponseJson>(checkPasswordResult.Value);

            Assert.True(response.Success == "true");

            Helper.DeleteUser(ctx.username);
        }

        public T DeserializeResponse<T>(object value)
        {
            T result;
            using (MemoryStream ms = new MemoryStream())
            {
                JsonSerializer.SerializeAsync(ms, value, typeof(object),
                    new JsonSerializerOptions() { }).GetAwaiter().GetResult();
                var jsonResulltString = Encoding.UTF8.GetString(ms.ToArray());
                result = JsonSerializer.Deserialize<T>(jsonResulltString);
            }

            return result;
        }

        [Fact]
        public void CheckPassword_Fail()
        {
            var ctx = Helper.RegisterUser(addIdentity: true);

            var sm = Helper.GetBackendService<SignInManager<ApplicationUser>>();
            var users = Helper.GetBackendService<IUserRepository>();
            JsonResult checkPasswordResult =
                (JsonResult)
                (ctx.controller.CheckPassword(ctx.password, sm, users)
                .GetAwaiter()
                .GetResult());

            ApiResponseJson response =
                DeserializeResponse<ApiResponseJson>(checkPasswordResult.Value);

            Assert.True(response.Success == "false");

            Helper.DeleteUser(ctx.username);
        }
    }
}