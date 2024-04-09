namespace  DayCatFacts.Plugins;

using System.ComponentModel;
using Microsoft.SemanticKernel;

using WorldTime;
using WorldTime.Config;
using Environment = WorldTime.Http.Environment;

/// <summary>
/// A Semantic Kernel plugin for retrieving current date and time information.
/// </summary>
public class DateTimePlugin
{
    private readonly WorldTimeClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimePlugin"/> class.
    /// This creates an SDK client to access the World Time API.
    /// </summary>
    public DateTimePlugin()
    {
        var config = new WorldTimeConfig { Environment = Environment.Default };
        client = new WorldTimeClient(config);
    }

    /// <summary>
    /// Retrieves the current date and time from the World Time API based on the current IP address.
    /// </summary>
    /// <returns>The current date and time as local time.</returns>
    [KernelFunction]
    [Description("Gets current date and time.")]
    public async Task<DateTime> GetDateandTime()
    {        
        Console.WriteLine("DayPlugin > Getting the date and time from the World Time API...");

        // Get the date and time based off the current IP address
        var response = await client.Ip.GetIpAsync();

        // Get the timezone
        var tzi = TimeZoneInfo.FindSystemTimeZoneById(response.Timezone);

        // Return the date and time as local
        return TimeZoneInfo.ConvertTime(DateTime.Parse(response.Datetime), tzi);
    }

    /// <summary>
    /// Retrieves the current time zone from the World Time API based on the current IP address.
    /// </summary>
    /// <returns>The <see cref="TimeZoneInfo"/> object representing the current time zone.</returns>
    [KernelFunction]
    [Description("Gets current time zone.")]

    public async Task<TimeZoneInfo> GetTimeZone()
    {        
        Console.WriteLine("DayPlugin > Getting the time zone from the World Time API...");

        // Get the timezone based off the current IP address
        var response = await client.Ip.GetIpAsync();

        // Convert the timezone to a TimeZoneInfo object
        return TimeZoneInfo.FindSystemTimeZoneById(response.Timezone);
    }
}