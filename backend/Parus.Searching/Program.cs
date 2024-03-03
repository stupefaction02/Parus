using Parus.Core.Services.ElasticSearch;
using Microsoft.AspNetCore.Mvc;

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

app.MapGet("/{q}", async (string q, int page, int size, 
    [FromServices] ElasticSearchService searchingService) =>
{
    int start = page == 0 ? 1 : size * (page - 1);
    int count = size == 0 ? 5 : size;

    var broadcasts = await searchingService.SearchBroadcastsByTitleTagsJsonAsync(q, start, count);
    var users = await searchingService.SearchUsersByUsernameJsonAsync(q, start, count);
    var categories = await searchingService.SearchCategoriesByNameJsonAsync(q, start, count);

    return new { 
        broadcast = broadcasts,
        users = users,
        categories = categories,
    };
});

app.Run();