using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Hosting;

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

            application.MapPost("/uploadManifest", async (IFormFile file, string usrDirectory, IWebHostEnvironment env) => { 
                bool directoryExists = false;

                string contentRoot = env.WebRootPath;
                string directoryPath = Path.Combine(contentRoot, "live", usrDirectory);
                // TODO: replace Path.COmbine with your own
                Directory.CreateDirectory(directoryPath);

                string fn = Path.Combine(directoryPath, "master_playlist.m3u8");
                using (FileStream destFs = File.Create(fn))
                {
                    Stream inputFs = file.OpenReadStream();
                    inputFs.Seek(0, SeekOrigin.Begin);

                    Console.WriteLine($"Uploading {file.FileName} file to {fn}");
                    await inputFs.CopyToAsync(destFs);
                }
            });

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
