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

namespace Naturistic.WebUI.Pages.Identity
{
    public class RecoveryModel : PageModel
    {
        private readonly ILogger<RegistrationModel> _logger;

        private readonly IApiClient apiClient;

        public RecoveryModel(ILogger<RegistrationModel> logger, IApiClient apiClient)
        {
            _logger = logger;
            this.apiClient = apiClient;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            

            // highlight error
            return null;
        }
    }
}
