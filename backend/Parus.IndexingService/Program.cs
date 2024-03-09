using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Parus.Core.Services.ElasticSearch.Indexing;
using Parus.IndexingService;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting ElasticIndexingEngine");

        string cf = "appsettings.json";
        string configPath = BuildConfigPath(cf);
        Console.WriteLine($"Loading config file from {configPath}");
        if (File.Exists(configPath))
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(cf);
            IConfigurationRoot configuration = builder.Build();

            try
            {
                (new ParusIndexingEngine(configuration)).Run();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Exception: {ex.GetType().Name}. Message: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR. Coulnd't found config file {cf}.");
            Console.ForegroundColor = ConsoleColor.White;
        }

        string BuildConfigPath(string cf)
        {
            string bin = System.Environment.CurrentDirectory;
            return Path.Combine(bin, cf);
        }
    }
}