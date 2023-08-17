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
using Naturistic.Common;

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

            if (jwtSignInResult == null)
            {
				logger.LogError($"Can't rerieve JWT Token. Contact the API server.");
				// TODO: Redirect to page explaining the isssue
			}

			ApiServerResponse response = await ServerUtils.ConvertResponseStream<ApiServerResponse>(jwtSignInResult);

            if (response != null)
            {
				logger.LogError($"Can't rerieve JWT Token. Contact the API server.");
			}

			if (response.Success == "Y")
            {
                Response.Cookies.Append("JWT", response.Payload);

                var user = await userManager.FindByNameAsync(nickname);

                string errorMessage = $"Wrong password for user {user.UserName}";
                logger.LogInformation($"Failed to login {nickname}. Error: {errorMessage}");
            }

            // highlight error
            return null;
        }
    }
}
