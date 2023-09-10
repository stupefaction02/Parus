using Microsoft.AspNetCore.Builder;

namespace Naturistic.Hsl
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            { WebRootPath = "Data" });

            WebApplication application = builder.Build();

            application.UseHttpsRedirection();
            application.UseStaticFiles();

            //application.MapPost("/upload", UploadHandler);

            application.Run();
        }
    }
}
