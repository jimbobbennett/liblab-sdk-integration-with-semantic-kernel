using static System.Console;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

using DayCatFacts;
using DayCatFacts.Plugins;

// Load the API key and selected model from the appsettings.json file
var appSettingsReader = new AppSettingsReader();
var openAISettings = appSettingsReader.ReadSection<OpenAISettings>("OpenAI");

// Create the kernel builder with OpenAI chat completion
var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(openAISettings.ChatModel, openAISettings.Key);

// Add plugins to the kernel
builder.Plugins.AddFromType<DateTimePlugin>();
builder.Plugins.AddFromType<CatFactPlugin>();
builder.Plugins.AddFromObject(new DallEPlugin(openAISettings.Key, openAISettings.ImageModel));

// Build the kernel
Kernel kernel = builder.Build();

// Create a chat history object
var history = new ChatHistory();

// Get chat completion service
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Start the conversation
WriteLine("I am an AI assistant who also knows the current day and time, and a load of cat facts!");
Write("User > ");
string userInput;

// Get user input and keep the conversation going until the user enters a blank line
while (!string.IsNullOrWhiteSpace(userInput = ReadLine()))
{
    // Add the user input to the history
    history.AddUserMessage(userInput);

    // Enable auto function calling
    OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    // Get the response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

    // Print the results
    WriteLine("Assistant > " + result);

    // Add the message from the agent to the chat history
    history.AddMessage(result.Role, result.Content ?? string.Empty);

    // Get user input again
    Write("User > ");
}