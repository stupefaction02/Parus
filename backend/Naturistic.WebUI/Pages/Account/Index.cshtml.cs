using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Parus.WebUI.Services;
using Parus.Infrastructure.Identity;

namespace Parus.WebUI.Pages.Account
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

		private readonly IApiClient apiClient;

        public IndexModel(ILogger<IndexModel> logger, IApiClient apiClient)
        {
            _logger = logger;
			this.apiClient = apiClient;
        }

        public IActionResult OnGet()
        {
			return Page();
        }
    }
}
