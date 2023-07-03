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

namespace Naturistic.WebUI.Pages.Identity.Login
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;

		private readonly IApiClient apiClient;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public IndexModel(ILogger<IndexModel> logger, IApiClient apiClient,
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
            var user = await userManager.FindByNameAsync(nickname);

            var signInResult = await signInManager.PasswordSignInAsync(user.UserName, password, false, false);

            if (signInResult.Succeeded)
            {
                logger.LogInformation($"{nickname} login successfully!");

                return StatusCode(200);
            }

            string errorMessage = $"Wrong password for user {user.UserName}";
            logger.LogInformation($"Failed to login {nickname}. Error: {errorMessage}");

            // highlight error
            return null;
        }
    }
}
