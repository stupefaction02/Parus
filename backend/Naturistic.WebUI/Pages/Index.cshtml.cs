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
    public class IndexModel : PageModel
    {
        public const int PAGE_SIZE = 12;

        public int PageCount { get; set; }

        public int Page { get; set; }

        public IEnumerable<BroadcastInfo> Broadcasts { get; set; }

        public IActionResult OnGet([FromQuery] string page, string search,
            [FromServices] IBroadcastInfoRepository broadcastInfoRepository)
        {
            PageCount = (broadcastInfoRepository.Count() / PAGE_SIZE) + 1;

            int pageInt32;
            if (!Int32.TryParse(page, out pageInt32))
            {
                pageInt32 = 1;
            }

            Page = pageInt32;

            int start = (pageInt32 - 1) * PAGE_SIZE;
            Broadcasts = broadcastInfoRepository.GetInterval(start, count: PAGE_SIZE);

            return Page();
        }
    }
}
