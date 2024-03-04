using Microsoft.IdentityModel.Tokens;
using Parus.Core.Interfaces;
using Parus.Core.Services.Localization;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Parus.Identity.Middlewares;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

IServiceCollection services = builder.Services;

services.AddRazorPages();

services.AddSignalR();

services.ConfigureSqlDatabase(configuration);

services.ConfigureCors();

services.ConfigureIdentity();

services.ConfigureRepositories();

services.AddJwtAuthentication(configuration);

services.AddMail(configuration);

services.AddResponseCompression();

services.AddTransient<ILocalizationService, LocalizationService>();

WebApplication app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseWebSockets();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(options =>
{
    options.WithOrigins("https://localhost:5002")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .SetIsOriginAllowed((x) => true)
               .AllowCredentials();
});

app.UseAuthentication();

app.UseMiddleware<CheckingLoggingInMiddleware>();

app.UseAuthorization();

app.UseMiddleware<DebugMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    //MapCustomRoutes(endpoints);
});

app.MapGet("/weatherforecast", () =>
{
    return "1";
});

app.Run();