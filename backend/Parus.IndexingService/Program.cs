using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Parus.Core.Services.ElasticSearch.Indexing;
using Parus.IndexingService;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Starting ElasticIndexingEngine");

        string cf = "C:\\Users\\Ivan\\Desktop\\sensorium\\NET Projects\\ASPNET\\Parus\\backend\\Parus.IndexingService\\appsettings.json";

        if (File.Exists(cf))
        {
            IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile(cf);
            IConfigurationRoot configuration = builder.Build();

            ParusIndexingEngine engine = new ParusIndexingEngine(configuration);

            engine.Run();
        }
    }
}