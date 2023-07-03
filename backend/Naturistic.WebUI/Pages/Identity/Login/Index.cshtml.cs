using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

using Naturistic.WebUI.Services;
using Naturistic.Infrastructure.Identity;
using System.Net.Http;

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
            Console.WriteLine("Log In...");

            var response = await apiClient.LoginAsync(Request.Form["nickname"], Request.Form["password"]);
            
            if (response is HttpResponseMessage httpResponse)
            {
                IEnumerable<string> vals;
                httpResponse.Headers.TryGetValues("Set-Cookie", out vals);

                var identityCookie = vals.First();

                Response.Headers.Add("Set-Cookie", identityCookie);
                Request.Headers.Add("Set-Cookie", identityCookie);

                foreach (var header in httpResponse.Content.Headers)
                {
                    Console.WriteLine(header.Key + " " + header.Value);
                }

                if (httpResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Log In API Call succeded");

                    return RedirectToPage("./Index");
                }
            }

            Console.WriteLine($"Log In API Call failed");

            // highlight error
            return null;
        }
    }
}
