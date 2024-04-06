namespace  DayCatFacts;

/// <summary>
/// Represents the settings for OpenAI.
/// </summary>
public class OpenAISettings
{
    /// <summary>
    /// Gets or sets the API key for OpenAI.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Gets or sets the model for OpenAI.
    /// </summary>
    public required string Model { get; set; }
}