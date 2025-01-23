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
using Parus.Core.Interfaces;

namespace Parus.WebUI.Pages.Broadcasts
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;

		private readonly IUserRepository userRepository;
        private readonly ILocalizationService localization;

        [BindProperty(SupportsGet = true)]
        public string BroadcastName { get; set; }

        public string UserColor { get; set; }

        public bool IsUserEmailConfirmed { get; set; }

        public IndexModel(ILogger<IndexModel> logger, 
            IUserRepository userRepository, 
            ILocalizationService localization)
        {
            this.logger = logger;
			this.userRepository = userRepository;
            this.localization = localization;
        }

        public PageResult OnGet()
        {
            var broadcaster = userRepository.One(x => x.GetUsername() == BroadcastName);

            if (broadcaster != null)
            {
                // If we want to bind a chat entity with other than broadcaster's username (uid for example)
                // or if username in url is not what we want to bind chat entity with
                // here we got all the information about broadcaster
                //this.HttpContext.Response.Cookies.Append("chatname", broadcaster.GetId());
                this.HttpContext.Response.Cookies.Append("chatid", broadcaster.GetUsername());

                string locale = this.HttpContext.Request.Cookies["locale"];
                localization.SetLocale(locale);
            }
            else
            {
                // TODO: redirect to "This broadcaster doesn't exist" page
            }

            string usernmae = User.Identity.Name;
			ParusUser usr = userRepository.One(x => x.GetUsername() == usernmae) as ParusUser;

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
