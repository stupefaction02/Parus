using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parus.Core.Interfaces;
using Parus.Core.Services.Localization;
using Microsoft.AspNetCore.Routing;
using System;
using Parus.Core.Interfaces.Services;
using Parus.Core.Services.ElasticSearch;
using Microsoft.AspNetCore.Mvc;
using Parus.Core.Entities;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;
ConfigureElastic(builder.Services, configuration);

void ConfigureElastic(IServiceCollection services, IConfiguration configuration)
{
    string basicAuthToken = configuration["Elastic:Auth:Basic"];
    string user = configuration["Elastic:Auth:User"];
    string host = configuration["Elastic:Host"];

    ElasticTransport instance = new ElasticTransport(host, (user, basicAuthToken));
    builder.Services.AddSingleton(instance);
    builder.Services.AddSingleton<ElasticSearchService>();
}

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/{q}", async (string q, [FromServices] ElasticSearchService searchingService) =>
{
    var broadcasts = await searchingService.SearchBroadcastsByTitleTagsAsync(q, 0, 8);
    //int broadcastsTotal = searchingService.CountBroadcastsByTitleTags(q);

    //IEnumerable<BroadcastCategory> categories = searchingService.SearchCategoryByName(q, 0, 5);
    //int categoriesTotal = searchingService.CountCategoriesByName(q);

    //IEnumerable<IUser> users = searchingService.SearchUsersByName(q, 0, 5);
    //int usersTotal = searchingService.CountUsersByName(q);

    //var ret = new 
    //{ 
    //    broadcast = broadcasts,
    //    broadcast_total = broadcastsTotal,
    //};

    //return ret;
});

app.Run();