using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Naturistic.WebUI.Services;
using Naturistic.Infrastructure.Identity;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;

namespace Naturistic.WebUI.Pages.Broadcasts
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;

		private readonly UserManager<ApplicationUser> userManager;

		[BindProperty(SupportsGet = true)]
        public string BroadcastName { get; set; }

        public bool IsUserEmailConfirmed { get; set; }

        public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager)
        {
            this.logger = logger;
			this.userManager = userManager;
		}

        public async Task<IActionResult> OnGet()
        {
			ApplicationUser usr = await userManager.GetUserAsync(User);

            if (usr != null)
            {
				// here user is registered but can be not confirmed
				IsUserEmailConfirmed = await userManager.IsEmailConfirmedAsync(usr);
				this.Response.Cookies.Append("email", usr.Email);
			}

            // if user is not anonymous
			if (User.Identity.Name != null)
            {
				this.Response.Cookies.Append("username", User.Identity.Name);
			}

			return Page();
        }

        public void OnGetCurrentUser()
        {
            Debug.WriteLine("Getting current user...");
        }
    }
}
