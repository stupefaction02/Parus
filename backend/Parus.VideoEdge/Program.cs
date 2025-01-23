using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

namespace Parus.VideoEdge
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
            Console.Title = "VideoEdge";

            WebApplicationBuilder builder = WebApplication.CreateBuilder(
                new WebApplicationOptions { WebRootPath = "Data" });

            builder.Services.AddCors();
            
            WebApplication application = builder.Build();
            
            builder.ConfigureKestrel();

            //application.UseCors(builder => builder.AllowAnyOrigin()
            //    .WithOrigins("https://localhost:5002").WithOrigins("http://127.0.0.1:8080"));

            application.UseCors(builder => builder.AllowAnyOrigin());

            //application.UseHttpsRedirection();

            //application.UseHslStaticFiles();

            application.UseStaticFiles(new StaticFileOptions { ServeUnknownFileTypes = true });

            string endPlaylistLine = "#EXT-X-ENDLIST";
            string liveDirName = "live";
            string masterPlaylistCommonName = "master_playlist.m3u8";
            string playlistCommonName = "playlist.m3u8";
            IWebHostEnvironment webHostEnvironment = application.Environment;
            string contentRoot = webHostEnvironment.WebRootPath;
            string liveDir = Path.Combine(contentRoot, liveDirName);

            application.MapGet("/hello", HelloWorld);

            application.MapPost("/uploadManifest", async (IFormFile file, string usrDirectory) => { 

                if (file == null) { return; }

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

            application.MapPost("/uploadSegment", async (HttpContext ctx, IFormFile file, string usrDirectory) => {

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
                    //await inputFs.CopyToAsync(destFs);
                }

                //LogHeaders(ctx.Request);
            });

            application.MapPost("/uploadPlaylists", async (HttpContext ctx, string usrDirectory) => {
                string directoryPath = PathCombine(liveDir, usrDirectory);

                string[] dirs = GetOrCreateQualityOptionsDirectories(directoryPath);

                long totalLength = 0;

                using (StreamReader reader = new StreamReader(ctx.Request.Body, Encoding.UTF8))
                {
                    // skip headers 
                    for (int i = 0; i < 4; i++)
                    {
                        await reader.ReadLineAsync();
                    }

                    string line = null;
                    foreach (string qdir in dirs)
                    {
                        string p = PathCombine(directoryPath, qdir);

                        using FileStream fs = File.Create( PathCombine(p, playlistCommonName) );
                        using StreamWriter sw = new StreamWriter(fs);

                        while (true)
                        {
                            line = await reader.ReadLineAsync();
#if DEBUG
                            totalLength += Encoding.UTF8.GetByteCount(line);
#endif
                            sw.WriteLine(line);

                            if (line == endPlaylistLine)
                            {
                                line = null;

                                await sw.FlushAsync();

                                break;
                            }
                        }
                    }
                }

#if DEBUG
                long kbs = totalLength / (long)1024;
                Console.Write($". Total size: {kbs} kbs" + Environment.NewLine);
#endif
            });

            application.Run();
        }

        private static void LogHeaders(HttpRequest request)
        {
            foreach (var h in request.Headers)
            {
                Console.WriteLine($"{h.Key}:{h.Value}");
            }
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

        public static string HelloWorld() => "Hello World!";
    }

    public static class WebApplicationExtensions
    {
        public static void ConfigureKestrel(this WebApplicationBuilder builder)
        {
            //builder.WebHost.ConfigureKestrel(x => { 
                
            //});
        }

        public static void UseHslStaticFiles(this WebApplication webApplication)
        {
            // defauls settings doon't have m3u8 as known file extension
            // https://source.dot.net/#Microsoft.AspNetCore.StaticFiles/FileExtensionContentTypeProvider.cs,6aaab7e19cea3805,references
            // use this
            //application.UseStaticFiles(new StaticFileOptions { ServeUnknownFileTypes = true });
            // or this
            FileExtensionContentTypeProvider fectp = new FileExtensionContentTypeProvider();
            fectp.Mappings.Add(".m3u8", "text/plain");
            fectp.Mappings.Add(".mpd", "text/plain");
            fectp.Mappings.Add(".mv4", "text/plain");

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
                    else if (ext == ".m4v")
                    {
                        context.Context.Response.Headers.ContentType = "video/mp4";
                        context.Context.Response.Headers.AcceptRanges = "bytes";
                        context.Context.Response.Headers.CacheControl = "max-age=29727833"; 
                        context.Context.Response.Headers.AccessControlMaxAge = "86400";
                        context.Context.Response.Headers.AccessControlAllowCredentials = "true";
                    }
                    else if (ext == ".mpd")
                    {
                        context.Context.Response.Headers.ContentType = "application/dash+xml";
                    }
                }
            };

            webApplication.UseStaticFiles(options);
        }
    }
}
