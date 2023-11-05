using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Parus.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Parus.Core.Interfaces;
using Parus.Core.Entities;
using Parus.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Parus.WebUI.Pages.Identity
{
	public class EditPasswordModelLocalizationGlossary
    {
        public static string EDITPSWRD_P1P1 => "EDITPSWRD_P1P1";
        public static string EDITPSWRD_P1P2 => "EDITPSWRD_P1P2";
        public static string EDITPSWRD_P1P3 => "EDITPSWRD_P1P3";
        public static string COMPLETE_BTN => "COMPLETE_BTN";
        public static string EDITPSWRD_WEAKPASSWORD => "EDITPSWRD_WEAKPASSWORD";
        public static string EDITPSWRD_NOMATCH => "EDITPSWRD_NOMATCH";
    }

    public class EditPasswordModel : PageModel
    {
        public string EDITPSWRD_P1P1 { get; set; }
        public string EDITPSWRD_P1P2 { get; set; }
        public string EDITPSWRD_P1P3 { get; set; }
        public string COMPLETE_BTN { get; set; }
        public string EDITPSWRD_WEAKPASSWORD { get; set; }
        public string EDITPSWRD_NOMATCH { get; set; }

        private readonly ILogger<EditPasswordModel> _logger;
		private readonly IUserRepository users;
		private readonly ILocalizationService localizationService;
		private readonly IPasswordRecoveryTokensRepository tokens;
		private readonly UserManager<ApplicationUser> userManager;
		private readonly IPasswordHasher<ApplicationUser> passwordHasher;

		public EditPasswordModel(IUserRepository users,
            ILocalizationService localizationService,
			IPasswordRecoveryTokensRepository passwordRecoveryTokensRepository,
			UserManager<ApplicationUser> userManager,
            IPasswordHasher<ApplicationUser> passwordHasher,
			ILogger<EditPasswordModel> logger)
        {
			this.users = users;
			this.localizationService = localizationService;
			this.tokens = passwordRecoveryTokensRepository;
			this.userManager = userManager;
			this.passwordHasher = passwordHasher;
			_logger = logger;
		}

        public IActionResult OnGet(string token)
        {
            if (String.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            IUser user = tokens.GetUser(token);

            if (user == null)
            {
                return Unauthorized();
            }

            string locale = HttpContext.Request.Cookies["locale"];
			localizationService.SetLocale(locale);

			EDITPSWRD_P1P1 = localizationService.RetrievePhrase(EditPasswordModelLocalizationGlossary.EDITPSWRD_P1P1);
            EDITPSWRD_P1P2 = localizationService.RetrievePhrase(EditPasswordModelLocalizationGlossary.EDITPSWRD_P1P2);
            EDITPSWRD_P1P3 = localizationService.RetrievePhrase(EditPasswordModelLocalizationGlossary.EDITPSWRD_P1P3);
			COMPLETE_BTN = localizationService.RetrievePhrase(EditPasswordModelLocalizationGlossary.COMPLETE_BTN);
			EDITPSWRD_WEAKPASSWORD = localizationService.RetrievePhrase(EditPasswordModelLocalizationGlossary.EDITPSWRD_WEAKPASSWORD);
			EDITPSWRD_NOMATCH = localizationService.RetrievePhrase(EditPasswordModelLocalizationGlossary.EDITPSWRD_NOMATCH);

			return Page();
        }

		public async Task<IActionResult> OnPostAsync(string newpassword)
        {
            string username = HttpContext.User.Identity.Name;

            // TODO: Find the way to make sigh out here for multiple schemes
			Response.Cookies.Delete("JWT");
			Response.Cookies.Delete("identity.username");

			ApplicationUser user = (ApplicationUser)users.One(x => x.GetUsername() == username);

            if (user == null)
            {
				return BadRequest("");
			}

			string newpasswordHash = passwordHasher.HashPassword(user, newpassword);

            user.PasswordHash = newpasswordHash;

            users.Update(user);
			//var updateResult = await userManager.UpdateAsync(user);

			//if (updateResult.Succeeded)
   //         {
   //             tokens.Delete(user.PasswordRecoveryToken);

   //             return RedirectToPage("index");
			//}

            await tokens.DeleteAsync(user.PasswordRecoveryToken);

            return RedirectToPage("Index");
        }

    }
}
