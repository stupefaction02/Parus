using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.WebUI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;
        private readonly SignInManager<ApplicationUser> signInManager;

        public IndexModel(ILogger<IndexModel> logger,
            SignInManager<ApplicationUser> signInManager)
        {
            this.logger = logger;
            this.signInManager = signInManager;
        }
    }
}
