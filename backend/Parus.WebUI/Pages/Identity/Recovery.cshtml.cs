using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Parus.WebUI.Services;
using Parus.Infrastructure.Identity;
using Cassandra;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Parus.Core.Interfaces;

namespace Parus.WebUI.Pages.Identity
{
    public class RecoveryPageLocalizationGlossary
    {
        public static string RECOVERY_P1P1 => "RECOVERY_P1P1";
        public static string RECOVERY_P1P2 => "RECOVERY_P1P2";
        public static string BTN_CONT => "BTN_CONT";

        public static string RECOVERY_PHASE2_P1 => "RECOVERY_PHASE2_P1";
        public static string RECOVERY_PHASE2_P2 => "RECOVERY_PHASE2_P2";
        public static string RECOVERY_PHASE2_BTN => "RECOVERY_PHASE2_BTN";


        public static string RECOVERY_PHASE3_P1 => "RECOVERY_PHASE3_P1";
        public static string RECOVERY_PHASE3_BTN2 => "RECOVERY_PHASE3_BTN2";
        public static string RECOVERY_PHASE3_BTN1 => "RECOVERY_PHASE3_BTN1";
    }

    public class RecoveryModel : PageModel
    {
        public string RECOVERY_P1P1 { get; set; }
        public string RECOVERY_P1P2 { get; set; }
        public string BTN_CONT { get; set; }

        public string RECOVERY_PHASE2_P1 { get; set; }
        public string RECOVERY_PHASE2_P2 { get; set; }
        public string RECOVERY_PHASE2_BTN { get; set; }

        public string RECOVERY_PHASE3_P1 { get; set; }
        public string RECOVERY_PHASE3_BTN2 { get; set; }
        public string RECOVERY_PHASE3_BTN1 { get; set; }

        private readonly IApiClient apiClient;
		private readonly ILocalizationService localizationService;

		public RecoveryModel(IApiClient apiClient, ILocalizationService localizationService)
        {
            this.apiClient = apiClient;
			this.localizationService = localizationService;
		}

        public IActionResult OnGet([FromServices] ILocalizationService localizationService)
        {
            string locale = HttpContext.Request.Cookies["locale"];
			localizationService.SetLocale(locale);

			RECOVERY_P1P1 = localizationService.RetrievePhrase(RecoveryPageLocalizationGlossary.RECOVERY_P1P1);
			RECOVERY_P1P2 = localizationService.RetrievePhrase(RecoveryPageLocalizationGlossary.RECOVERY_P1P2);
			BTN_CONT = localizationService.RetrievePhrase(RecoveryPageLocalizationGlossary.BTN_CONT);

            RECOVERY_PHASE2_P1 = localizationService.RetrievePhrase(RecoveryPageLocalizationGlossary.RECOVERY_PHASE2_P1);
            RECOVERY_PHASE2_P2 = localizationService.RetrievePhrase(RecoveryPageLocalizationGlossary.RECOVERY_PHASE2_P2);
            RECOVERY_PHASE2_BTN = localizationService.RetrievePhrase(RecoveryPageLocalizationGlossary.RECOVERY_PHASE2_BTN);

            RECOVERY_PHASE3_P1 = localizationService.RetrievePhrase(RecoveryPageLocalizationGlossary.RECOVERY_PHASE3_P1);
            RECOVERY_PHASE3_BTN2 = localizationService.RetrievePhrase(RecoveryPageLocalizationGlossary.RECOVERY_PHASE3_BTN1);
            RECOVERY_PHASE3_BTN1 = localizationService.RetrievePhrase(RecoveryPageLocalizationGlossary.RECOVERY_PHASE3_BTN2);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
			return null;
        }
    }
}
