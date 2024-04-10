namespace  LLMAndSDKDemo;

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
    /// Gets or sets the chat model for OpenAI.
    /// </summary>
    public required string ChatModel { get; set; }

    /// <summary>
    /// Gets or sets the image model for OpenAI.
    /// </summary>
    public required string ImageModel { get; set; }
}