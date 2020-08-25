using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Naturistic.WebUI.Services;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.WebUI.Pages.Identity.Login
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
		
		public async Task<IActionResult> OnPostAsync()
		{
			Console.WriteLine("Login...");
			
			return RedirectToPage("./Index");
		}
    }
}
