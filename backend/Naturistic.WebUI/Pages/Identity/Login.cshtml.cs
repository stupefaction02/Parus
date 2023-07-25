using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Naturistic.WebUI.Services;
using Naturistic.Infrastructure.Identity;
using System.Net.Http;
using Cassandra;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Naturistic.Core;

using System.Text.Json;
using System.IO;

namespace Naturistic.WebUI.Pages.Identity
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<LoginModel> logger;

		private readonly IApiClient apiClient;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public LoginModel(ILogger<LoginModel> logger, IApiClient apiClient,
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager)
        {
            this.logger = logger;
			this.apiClient = apiClient;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public IActionResult OnGet()
        {
			return Page();
        }
		
		public async Task<IActionResult> OnPostAsync()
		{
			string nickname = Request.Form["nickname"];
            string password = Request.Form["password"];
            logger.LogInformation($"Attempt to login {nickname}");

            var jwtSignInResult = await apiClient.LoginJwtAsync(nickname, password) as HttpResponseMessage;
            //Console.WriteLine(jwtSignInResult.GetType().FullName);
            if (jwtSignInResult == null)
            {
				logger.LogError($"Can't rerieve JWT Token. Contact the API server.");
				// TODO: Redirect to page explaining the isssue
			}

			string jsonString;
            using (var inputStream = new StreamReader(jwtSignInResult.Content.ReadAsStream()))
            {
                jsonString = inputStream.ReadToEnd();
            }

            if (String.IsNullOrEmpty(jsonString))
            {
				logger.LogError($"Can't rerieve JWT Token. Contact the API server.");
				// TODO: Redirect to page explaining the isssue
			}

            var token = JsonSerializer.Deserialize<JwtToken>(jsonString);

            Request.HttpContext.Session.SetString("jwt.accessToken", token.AccessToken);
			
			var user = await userManager.FindByNameAsync(nickname);

            var signInResult = await signInManager.PasswordSignInAsync(user.UserName, password, false, false);

            if (signInResult.Succeeded)
            {
                logger.LogInformation($"{nickname} login successfully!");

                RedirectToPage("Index");

                return null;
            }

            string errorMessage = $"Wrong password for user {user.UserName}";
            logger.LogInformation($"Failed to login {nickname}. Error: {errorMessage}");

            // highlight error
            return null;
        }
    }
}
