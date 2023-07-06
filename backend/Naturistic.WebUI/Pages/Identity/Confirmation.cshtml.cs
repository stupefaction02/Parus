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

namespace Naturistic.WebUI.Pages.Identity
{
    public class ConfirmationModel : PageModel
    {
        private readonly ILogger<ConfirmationModel> logger;

		private readonly IApiClient apiClient;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public ConfirmationModel(ILogger<ConfirmationModel> logger, IApiClient apiClient,
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
            string first = Request.Form["1"];
            string second = Request.Form["2"];
            string third = Request.Form["3"];
            string fourth = Request.Form["4"];
            string fifth = Request.Form["5"];

            //logger.LogInformation($"Attempt to login {nickname}");
            //var user = await userManager.FindByNameAsync(nickname);

            //var signInResult = await signInManager.PasswordSignInAsync(user.UserName, password, false, false);

            //if (signInResult.Succeeded)
            //{
            //    logger.LogInformation($"{nickname} login successfully!");

            //    RedirectToPage("Index");

            //    return null;
            //}

            //string errorMessage = $"Wrong password for user {user.UserName}";
            //logger.LogInformation($"Failed to login {nickname}. Error: {errorMessage}");

            //// highlight error
            return null;
        }
    }
}
