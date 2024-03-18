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
using System.Drawing.Printing;
using Parus.Core.Services.ElasticSearch;
using System.Threading.Tasks;
using System.Linq;

namespace Parus.WebUI.Pages.Overview.Contexts
{
    public class OverviewSearchResultContext
    {
        public List<BroadcastInfoElasticDto> Broadcasts { get; set; }
        public int BroadcastsCount { get; set; }

        public List<BroadcastCategory> Categories { get; set; }
        public int CategoriesCount { get; set; }

        public List<UserElasticDto> Users { get; set; }
        public int UsersCount { get; set; }

        public string Query { get; set; }

        public OverviewSearchResultContext(ElasticSearchService searchingService, string q)
        {
            Query = q;

            // TODO: add ISearchTResult and conjuct MSSQsearch and ElasticDtos in one interface

            Task<Result> broadcastsTask = searchingService.SearchBroadcastsByTitleTagsAsync(q, 0, 8);

            Task<Result> categoriesTask = searchingService.SearchCategoriesByNameAsync(q, 0, 5);

            Task<Result> usersTask = searchingService.SearchUsersByUsernameAsync(q, 0, 5);

            Task.WaitAll(broadcastsTask, categoriesTask, usersTask);

            Broadcasts = (List<BroadcastInfoElasticDto>)broadcastsTask.Result.Items;
            Categories = (List<BroadcastCategory>)categoriesTask.Result.Items;
            Users = (List<UserElasticDto>)usersTask.Result.Items;

            UsersCount = Users.Count;
            CategoriesCount = Categories.Count;
            BroadcastsCount = Broadcasts.Count;
        }

        public bool Any()
        {
            return (BroadcastsCount > 0) || (CategoriesCount > 0) || (UsersCount > 0);
        }
    }

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
        public IEnumerable<IUserSearchResult> Users { get; set; }

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
            this.Users = searchingService.SearchUsersByName(Query, start, PAGE_SIZE);

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
        public const int PAGE_SIZE = 12;

        public IEnumerable<IBroadcastsInfoSearchResult> Broadcasts { get; set; }

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

            int count = searchingService.CountUsersByName(Query);

            int start = (pageInt32 - 1) * PAGE_SIZE;
            this.Broadcasts = searchingService.SearchBroadcastsByTitleTags(Query, PAGE_SIZE);

            int pageCount = (count / PAGE_SIZE) + 1;

            Pagination = new PaginationContext { Page = pageInt32, PageCount = pageCount };

#if DEBUG
            sw.Stop();
            Console.WriteLine($"Searching. Users. {sw.ElapsedMilliseconds} ms");
#endif
        }
    }

    public class AllCategoiresSearchResultContext : SearchResultContext
    {
        public const int PAGE_SIZE = 12;

        public IEnumerable<BroadcastCategory> Categories { get; set; }

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

            int start = (pageInt32 - 1) * PAGE_SIZE;
            this.Categories = searchingService.SearchCategoryByName(Query, start, PAGE_SIZE);

            int pageCount = (usersCount / PAGE_SIZE) + 1;

            Pagination = new PaginationContext { Page = pageInt32, PageCount = pageCount };

#if DEBUG
            sw.Stop();
            Console.WriteLine($"Searching. Users. {sw.ElapsedMilliseconds} ms");
#endif
        }
    }
}