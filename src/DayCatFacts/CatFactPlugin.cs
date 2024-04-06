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
    private readonly CatFactsClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="CatFactPlugin"/> class with the specified API key.
    /// This creates an SDK client to access the Cat Fact API using the provided API key.
    /// </summary>
    /// <param name="apiKey">The API key used for authentication with the Cat Facts service.</param>
    public CatFactPlugin(string apiKey)
    {
        var config = new CatFactsConfig { Environment = Environment.Default, ApiKeyAuth = new ApiKeyAuthConfig(apiKey) };
        _client = new CatFactsClient(config);
    }

    /// <summary>
    /// Retrieves a random cat fact from the Cat Fact API.
    /// </summary>
    /// <returns>A string representing a cat fact.</returns>
    [KernelFunction]
    [Description("Gets a cat fact.")]
    public async Task<string> GetCatFact()
    {
        Console.WriteLine("Getting cat fact from the Cat Facts API...");

        var response = await _client.Facts.GetRandomFactAsync();
        return response.Fact;
    }
}