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

namespace Naturistic.WebUI.Pages.Broadcasts
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;

		private readonly IApiClient apiClient;

        [BindProperty(SupportsGet = true)]
        public string BroadcastName { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IApiClient apiClient)
        {
            this.logger = logger;
			this.apiClient = apiClient;
        }

        public IActionResult OnGet()
        {
			return Page();
        }

        public void OnGetCurrentUser()
        {
            Debug.WriteLine("Getting current user...");
        }
    }
}
