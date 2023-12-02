using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Parus.WebUI.Services;
using Parus.Infrastructure.Identity;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Parus.Core.Interfaces.Repositories;

namespace Parus.WebUI.Pages.Broadcasts
{
    public class Index1 : PageModel
    {
        private readonly ILogger<Index1> logger;

		private readonly IUserRepository userRepository;

		[BindProperty(SupportsGet = true)]
        public string BroadcastName { get; set; }

        public string UserColor { get; set; }

        public bool IsUserEmailConfirmed { get; set; }

        public Index1(ILogger<Index1> logger, IUserRepository userRepository)
        {
            this.logger = logger;
			this.userRepository = userRepository;
		}

        public PageResult OnGet()
        {
            string usernmae = User.Identity.Name;
			ApplicationUser usr = userRepository.One(x => x.GetUsername() == usernmae) as ApplicationUser;

            if (usr != null)
            {
				// here user is registered but can be not confirmed
				IsUserEmailConfirmed = usr.EmailConfirmed;
				this.Response.Cookies.Append("email", usr.Email);

                UserColor = usr.ChatColor;
            }

            // if user is not anonymous
			if (!String.IsNullOrEmpty(usernmae))
            {
				this.Response.Cookies.Append("username", User.Identity.Name);
			}

			return Page();
        }
    }
}
