using Parus.Infrastructure.DLA;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Parus.Backend.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http.Json;
using Parus.Core;

namespace Parus.API.Tests
{

    public class RegistrationTests : IClassFixture<BackendApiTestsFixture>
    {
        private readonly string usersApiPath = "api/account";
        private readonly BackendApiTestsFixture _fixture;

        public RegistrationTests(BackendApiTestsFixture fixture)
        {
            _fixture = fixture;
        }

        // Form of seeding AND test at the same time :P
        [Fact]
        public async Task Seed_Database()
        {
            _fixture.ResetDatabase();

            Action addUserWhoWantsChangeHisPasswordVeryBadly01 = new Action(async () => {
                string username = "UserWhoWantsChangeHisPasswordVeryBadly01";
                string email = "UserWhoWantsChangeHisPasswordVeryBadly01@gmail.com".ToLower();
                string oldPassword = "zx1";

                string url = usersApiPath + $"/register?username={username}&email={email}&password={oldPassword}&gender=1";

                var response = await _fixture.Client.PostAsync(url, new StringContent("c"));
            });

            addUserWhoWantsChangeHisPasswordVeryBadly01.Invoke();

            _fixture.AddSeederActions(addUserWhoWantsChangeHisPasswordVeryBadly01);
        }

        [Fact]
        public async Task Ensure_Server_Controllers_Routing_Is_Ok()
        {
            var response = await _fixture.Client.GetAsync("api/hellojson");

            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            var responseJson = await response.FromJsonAsync<ApiResponseJson>();

            Assert.NotNull(responseJson);
            Assert.True(responseJson.Message == "Hello World!");
            Assert.True(responseJson.Success == "true");
        }

        [Fact]
        public async Task Register_User()
        {
            _fixture.ResetDatabase();

            // Arrange
            string a = Guid.NewGuid().ToString().Substring(0, 8);
            string username = "test_ivan73_" + a;
            string email = "testivan73" + a + "@gmail.com";
            string password = "zx1";

            // Action
            string url = usersApiPath + $"/register?username={username}&email={email}&password={password}&gender=1";

            var response = await _fixture.Client.PostAsync(url, new StringContent("c"));

            // Validation
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            var responseJson = await response.FromJsonAsync<ApiResponseJson>();

            Assert.True(responseJson.Success == "true");

            var newUser = _fixture.Database.Users.SingleOrDefaultAsync(x => x.UserName == username);

            Assert.NotNull(newUser);
        }

        [Fact]
        public async Task Change_Password()
        {
            // Arangre
            _fixture.ResetDatabase();

            string username = "UserWhoWantsChangeHisPasswordVeryBadly01";
            var userWhoWantsToChangePassword = _fixture.Database.Users.SingleOrDefault(x => x.UserName == username);

            string newPassword = "zx12";

            // Arangre
            //string url = usersApiPath + $"/register?username={username}&email={email}&password={password}&gender=1";

            //var response = await _fixture.Client.PostAsync(url, new StringContent("c"));

            //Assert.True(response.IsSuccessStatusCode);
            //Assert.NotNull(response.Content);

            //var responseJson = await response.FromJsonAsync<ApiResponseJson>();

            //Assert.True(responseJson.Success == "true");
        }

        [Fact]
        public async Task Login_With_RefreshToken_Fail_And_Request_Another_AccessToken()
        {
            _fixture.ResetDatabase();

            string a = Guid.NewGuid().ToString().Substring(0, 8);
            string username = "test_ivan73_" + a;
            string email = "testivan73" + a + "@gmail.com";
            string password = "zx1";

            string url = usersApiPath + $"/register?username={username}&email={email}&password={password}&gender=1";

            var response = await _fixture.Client.PostAsync(url, new StringContent("c"));

            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);

            var responseJson = await response.FromJsonAsync<ApiResponseJson>();

            Assert.True(responseJson.Success == "true");
        }
    }

    public static class HttpResponseExtensions
    {
        public static async Task<T> FromJsonAsync<T>(this HttpResponseMessage response)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}