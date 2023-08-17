using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Naturistic.WebUI.Services;
using Naturistic.Infrastructure.Identity;
using Cassandra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Naturistic.Core.Interfaces;
using Naturistic.Core.Entities;

using Naturistic.Infrastructure;
using Naturistic.Infrastructure.DLA.Repositories;
using Naturistic.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Naturistic.WebUI.Pages.Identity
{
    public class EditPasswordModelLocalizationGlossary
    {
        public static string EDITPSWRD_P1P1 => "EDITPSWRD_P1P1";
        public static string EDITPSWRD_P1P2 => "EDITPSWRD_P1P2";
        public static string EDITPSWRD_P1P3 => "EDITPSWRD_P1P3";
        public static string COMPLETE_BTN => "COMPLETE_BTN";
    }

    public class EditPasswordModel : PageModel
    {
        public string EDITPSWRD_P1P1 { get; set; }
        public string EDITPSWRD_P1P2 { get; set; }
        public string EDITPSWRD_P1P3 { get; set; }
        public string COMPLETE_BTN { get; set; }

        private readonly ILogger<EditPasswordModel> _logger;

		private readonly ILocalizationService localizationService;

		public EditPasswordModel(ILogger<EditPasswordModel> logger)
        {
            _logger = logger;
		}

        public IActionResult OnGet([FromServices] ILocalizationService localizationService,
            [FromServices] IPasswordRecoveryTokensRepository passwordRecoveryTokensRepository,
            string token)
        {
            if (String.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            IUser user = passwordRecoveryTokensRepository.GetUser(token);

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

			return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
			return null;
        }
    }
}
