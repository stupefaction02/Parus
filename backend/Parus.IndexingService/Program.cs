using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Parus.Core.Services.ElasticSearch.Indexing;
using Parus.IndexingService;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting ElasticIndexingEngine");

        Console.Title = "Indexing";

        string cf = "indexing_settings.json";
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
            catch (AggregateException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Exception: {ex.GetType().Name}.");

                Console.WriteLine($"Inner exceptions:");
                foreach (var sex in ex.InnerExceptions)
                {
                    Console.WriteLine($"Exception: {sex.GetType().Name}. Message: {sex.Message}");
                }

                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (UriFormatException)
            {

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