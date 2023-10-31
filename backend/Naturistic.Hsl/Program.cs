using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Hosting;

namespace Naturistic.Hsl
{
    public class Program
    {
        static string[] qualityOptionsDirNames = new string[3] {
                "720",
                "360",
                "180"
            };

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


            string liveDirName = "live";
            string masterPlaylistCommonName = "master_playlist.m3u8";
            string playlistCommonName = "playlist.m3u8";
            IWebHostEnvironment webHostEnvironment = application.Environment;
            string contentRoot = webHostEnvironment.WebRootPath;
            string liveDir = Path.Combine(contentRoot, liveDirName);

            application.MapPost("/uploadManifest", async (IFormFile file, string usrDirectory) => { 
                bool directoryExists = false;

                string directoryPath = Path.Combine(liveDir, usrDirectory);
                // TODO: replace Path.COmbine with your own
                Directory.CreateDirectory(directoryPath);

                string fn = Path.Combine(directoryPath, masterPlaylistCommonName);
                using (FileStream destFs = File.Create(fn))
                {
                    Stream inputFs = file.OpenReadStream();
                    inputFs.Seek(0, SeekOrigin.Begin);

                    Console.WriteLine($"Uploading manifest {file.FileName} file as ~/{liveDirName}/{usrDirectory}/{masterPlaylistCommonName}");
                    await inputFs.CopyToAsync(destFs);
                }
            });

            // server itself set its own quality options (1080, 720, 420)

            application.MapPost("/uploadSegment", async (IFormFile file, string usrDirectory) => {

                string directoryPath = Path.Combine(liveDir, usrDirectory);
                // TODO: replace Path.COmbine with your own
                Directory.CreateDirectory(directoryPath);

                string fn = Path.Combine(directoryPath, file.FileName);
                using (FileStream destFs = File.Create(fn))
                {
                    Stream inputFs = file.OpenReadStream();
                    inputFs.Seek(0, SeekOrigin.Begin);

                    Console.Write($"Uploading segment {file.FileName} file as ~/{liveDirName}/{usrDirectory}/{file.FileName}");

                    long kbs = file.Length / (long)1024;
                    Console.Write($". Total size: {kbs} kbs" + Environment.NewLine);
                    await inputFs.CopyToAsync(destFs);
                }
            });

            application.MapPost("/uploadPlaylists", async (IFormFileCollection files, string usrDirectory) => {
                GetOrCreateQualityOptionsDirectories(usrDirectory);

                // /live/{userid}/
                string directoryPath = Path.Combine(liveDir, usrDirectory);

                long totalLength = 0;

                int qdi = 0;
                string[] dirs = GetOrCreateQualityOptionsDirectories(directoryPath);
                foreach (IFormFile file in files)
                {
                    // /live/{userid}/{quality}
                    string qualityDir = dirs[qdi];
                    qdi++;

                    // /live/{userid}/{quality}/playlist.m3u8
                    string fn = Path.Combine(qualityDir, playlistCommonName);
                    using (FileStream destFs = File.Create(fn))
                    {
                        Stream inputFs = file.OpenReadStream();
                        inputFs.Seek(0, SeekOrigin.Begin);

                        Console.Write($"Uploading playlist {file.FileName} file as ~/{liveDirName}/{usrDirectory}/{masterPlaylistCommonName}");
                        await inputFs.CopyToAsync(destFs);

                        totalLength += file.Length;
                    }
                }

                long kbs = totalLength / (long)1024;
                Console.Write($". Total size: {kbs} kbs" + Environment.NewLine);
            });

            application.Run();
        }

        private static string[] GetOrCreateQualityOptionsDirectories(string usrDirectory)
        {
            int index = 0;
            string[] ret = new string[qualityOptionsDirNames.Length];
            foreach (var dir in qualityOptionsDirNames)
            {
                ret[index] = Directory.CreateDirectory( PathCombine(usrDirectory, dir) ).FullName;
                index++;
            }

            return ret;
        }

        // TODO: replace Path.Combine with your own
        private static string PathCombine(string a, string b)
        {
            return Path.Combine(a, b);
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
