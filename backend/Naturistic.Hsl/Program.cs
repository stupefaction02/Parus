using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders.Physical;

namespace Naturistic.Hsl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(
                new WebApplicationOptions { WebRootPath = "Data" });

            builder.Services.AddCors();

            WebApplication application = builder.Build();

            application.UseCors(builder => builder.AllowAnyOrigin()
                .WithOrigins("https://localhost:5002").WithOrigins("http://127.0.0.1:8080"));
            application.UseHttpsRedirection();

            application.UseHslStaticFiles();

            application.Run();
        }
    }

    public static class WebApplicationExtensions
    {
        public static void UseHslStaticFiles(this WebApplication webApplication)
        {
            // defauls settings doon't have m3u8 as known file extension
            // https://source.dot.net/#Microsoft.AspNetCore.StaticFiles/FileExtensionContentTypeProvider.cs,6aaab7e19cea3805,references
            // use this
            //application.UseStaticFiles(new StaticFileOptions { ServeUnknownFileTypes = true });
            // or this
            FileExtensionContentTypeProvider fectp = new FileExtensionContentTypeProvider();
            fectp.Mappings.Add(".m3u8", "text/plain");

            StaticFileOptions options = new StaticFileOptions 
            { 
                ContentTypeProvider = fectp,
                OnPrepareResponse = (context) =>
                {
                    PhysicalFileInfo fi = context.File as PhysicalFileInfo;
                    string ext = Path.GetExtension(fi.Name);
                    if (ext == ".ts")
                    {
                        context.Context.Response.Headers.ContentType = "application/octet-stream";
                        context.Context.Response.Headers.CacheControl = "public, max-age=3600";
                        context.Context.Response.Headers.AcceptRanges = "bytes";
                    } 
                    else if (ext == "m3u8")
                    {

                    }
                }
            };

            webApplication.UseStaticFiles(options);
        }
    }
}
