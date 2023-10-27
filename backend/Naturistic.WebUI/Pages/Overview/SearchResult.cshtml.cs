using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using MimeKit.Cryptography;
using Naturistic.Core.Entities;
using Naturistic.Core.Interfaces;
using Naturistic.Core.Interfaces.Repositories;
using Naturistic.Core.Interfaces.Services;
using Naturistic.Infrastructure.Identity;

namespace Naturistic.WebUI.Pages
{
    public class SearchResultModel : PageModel
    {
        public IEnumerable<BroadcastInfo> Broadcasts { get; set; }
        public IEnumerable<BroadcastCategory> Categories { get; set; }

        public IActionResult OnGet([FromQuery] string q,
            [FromServices] ISearchingService searchingService)
        {
            Broadcasts = searchingService.SearchBroadcastsByTitleTags(q, 8);

            Categories = searchingService.SearchCategoryByName(q, 5);

            return Page();
        }
    }
}
