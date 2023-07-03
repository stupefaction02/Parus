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

namespace Naturistic.WebUI.Pages.Identity.Register
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
			Console.WriteLine("Register...");
            
            var response = await apiClient.RegisterAsync(Request.Form["nickname"],
                Request.Form["first_name"], Request.Form["last_name"], Request.Form["email"], "zx1");

            if (response is ObjectResult actionResultResponse)
            {
                if (actionResultResponse.StatusCode == 200)
                {
                    Console.WriteLine($"Register API Call Response: {actionResultResponse.Value}");

                    return RedirectToPage("./Index");
                }

                Console.WriteLine($"Register API Call Response: {actionResultResponse.Value}");
            }

            Console.WriteLine($"Register API Call failed");

            // highlight error
            return null;
        }
    }
}
