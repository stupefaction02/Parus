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

namespace Naturistic.WebUI.Pages.Identity
{
    public class RecoveryPageLocalizationGlossary
    {
        public static string RECOVERY_P1P1 => "RECOVERY_P1P1";
        public static string RECOVERY_P1P2 => "RECOVERY_P1P2";
        public static string BTN_CONT => "BTN_CONT";
    }

    public class RecoveryModel : PageModel
    {
        public string RECOVERY_P1P1 { get; set; }
        public string RECOVERY_P1P2 { get; set; }
        public string BTN_CONT { get; set; }

        private readonly ILogger<RegistrationModel> _logger;

        private readonly IApiClient apiClient;
		private readonly ILocalizationService localizationService;

		public RecoveryModel(ILogger<RegistrationModel> logger, IApiClient apiClient, ILocalizationService localizationService)
        {
            _logger = logger;
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

			return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
			return null;
        }
    }
}
