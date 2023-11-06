using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Parus.Core.Entities;
using Parus.Core.Interfaces;
using Parus.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace Parus.WebUI.Pages.Overview.Contexts
{
    public abstract class SearchResultContext
    {
        protected readonly string Query;

        public PaginationContext Pagination { get; protected set; }

        public SearchResultContext(string query)
        {
            this.Query = query;
        }

        public abstract void Init(ISearchingService searchingService, string page);
    }

    public class AllUsersSearchResultContext : SearchResultContext
    {
        public IEnumerable<IUser> Users { get; set; }

        public const int PAGE_SIZE = 12;

        public AllUsersSearchResultContext(string query) : base(query) { }

        public override void Init(ISearchingService searchingService, string page)
        {
#if DEBUG

            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            int pageInt32;
            if (!Int32.TryParse(page, out pageInt32))
            {
                pageInt32 = 1;
            }

            int usersCount = searchingService.CountUsersByName(Query);

            int start = (pageInt32 - 1) * PAGE_SIZE;
            this.Users = (IEnumerable<IUser>)searchingService.SearchUsersByName(Query, start, PAGE_SIZE);

            int pageCount = (usersCount / PaginationContext.PAGE_SIZE) + 1;

            Pagination = new PaginationContext { Page = pageInt32, PageCount = pageCount };

#if DEBUG
            sw.Stop();
            Console.WriteLine($"Searching. Users. {sw.ElapsedMilliseconds} ms");
#endif
        }
    }

    public class AllBroadcastSearchResultContext : SearchResultContext
    {
        public IEnumerable<IUser> Users { get; set; }

        public AllBroadcastSearchResultContext(string query) : base(query)
        {
        }

        public override void Init(ISearchingService searchingService, string page)
        {
#if DEBUG

            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            int pageInt32;
            if (!Int32.TryParse(page, out pageInt32))
            {
                pageInt32 = 1;
            }

            int usersCount = searchingService.CountUsersByName(Query);

            this.Users = (IEnumerable<IUser>)searchingService.SearchUsersByName(Query, PaginationContext.PAGE_SIZE);

            int pageCount = (usersCount / PaginationContext.PAGE_SIZE) + 1;

            Pagination = new PaginationContext { Page = pageInt32, PageCount = pageCount };

#if DEBUG
            sw.Stop();
            Console.WriteLine($"Searching. Users. {sw.ElapsedMilliseconds} ms");
#endif
        }
    }

    public class AllCategoiresSearchResultContext : SearchResultContext
    {
        public IEnumerable<IUser> Users { get; set; }

        public AllCategoiresSearchResultContext(string query) : base(query)
        {
        }

        public override void Init(ISearchingService searchingService, string page)
        {
#if DEBUG

            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif

            int pageInt32;
            if (!Int32.TryParse(page, out pageInt32))
            {
                pageInt32 = 1;
            }

            int usersCount = searchingService.CountUsersByName(Query);

            this.Users = (IEnumerable<IUser>)searchingService.SearchCategoryByName(Query, PaginationContext.PAGE_SIZE);

            int pageCount = (usersCount / PaginationContext.PAGE_SIZE) + 1;

            Pagination = new PaginationContext { Page = pageInt32, PageCount = pageCount };

#if DEBUG
            sw.Stop();
            Console.WriteLine($"Searching. Users. {sw.ElapsedMilliseconds} ms");
#endif
        }
    }
}