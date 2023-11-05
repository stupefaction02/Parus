using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace ImageServer
{
    public class Program
    {
        static string contentRoot;
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions { WebRootPath = "Data" });

            builder.Services.AddCors();

            WebApplication application = builder.Build();

            contentRoot = application.Environment.WebRootPath;

            application.UseCors(options => options.AllowAnyOrigin());

            application.UseHttpsRedirection();
            application.UseStaticFiles();

            application.MapPost("/upload", UploadHandler);

			application.Run();
        }

        private static async Task UploadHandler(HttpContext context, IFormFile file, [FromServices] IWebHostEnvironment env)
		{
            string fn = Path.Combine(contentRoot, "previews", file.FileName);
            using (FileStream destFs = File.Create(fn))
            {
                Stream inputFs = file.OpenReadStream();
                inputFs.Seek(0, SeekOrigin.Begin);

                Console.WriteLine($"Uploading {file.FileName} file as ~/previews/{file.FileName}");
                await inputFs.CopyToAsync(destFs);
            }
        }
	}
}
