namespace  DayCatFacts.Plugins;

using System.ComponentModel;

using CatFacts;
using CatFacts.Config;
using Microsoft.SemanticKernel;
using Environment = CatFacts.Http.Environment;

/// <summary>
/// A Semantic Kernel plugin for retrieving cat facts.
/// </summary>
public class CatFactPlugin
{
    private readonly CatFactsClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="CatFactPlugin"/> class.
    /// This creates an SDK client to access the Cat Fact API.
    /// </summary>
    public CatFactPlugin()
    {
        var config = new CatFactsConfig { Environment = Environment.Default };
        client = new CatFactsClient(config);
    }

    /// <summary>
    /// Retrieves a random cat fact from the Cat Fact API.
    /// </summary>
    /// <returns>A string representing a cat fact.</returns>
    [KernelFunction]
    [Description("Gets a cat fact.")]
    public async Task<string> GetCatFact()
    {
        Console.WriteLine("CatFactPlugin > Getting a cat fact from the Cat Facts API...");

        var response = await client.Facts.GetRandomFactAsync();

        Console.WriteLine("CatFactPlugin > Cat fact: " + response.Fact);

        return response.Fact;
    }
}