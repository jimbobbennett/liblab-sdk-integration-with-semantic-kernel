# Integrate liblab generated SDKs with your LLM using Semantic Kernel for RAG (retrieval augmented generation)

This repo shows you how you can integrate your own data sources into an LLM powered app for RAG, using auto-generated C# SDKs from [liblab](https://liblab.com), [Microsoft Semantic Kernel](https://learn.microsoft.com/semantic-kernel/) as an orchestrator, and [OpenAI](https://openai.com).

Semantic Kernel is a library that allows you use an LLM to call agents in your own code, using that data to augment the responses, or to run actions. This demo shows how you can quickly create a C# SDK for your APIs and integrate those SDKs into Semantic Kernel plugins, bringing your data into the LLM.

## Prerequisites

To build and run this demo you will need:

- Visual Studio or Visual Studio Code
- Either .NET 8.0 installed locally, or docker installed so you can use the included devcontainer (recommended)
- An OpenAI API key
- A [liblab](https://liblab.com) account

    If you don't have a liblab account, you can create one [from liblab.com by selecting **Get Started**](https://liblab.com). You can sign up for free to get 15 SDK builds.

## Integrated APIs

This demo integrates three APIs:

- [World Time API](http://worldtimeapi.org) - this is an API that can get the current date and time for you based off your IP address. LLMs don't know the current date and time, so access to this information can be useful. Although this is overkill to use an API instead of just getting the current date and time from the system, it's a good example of how you can integrate any API into your LLM.
- [Cat Facts API](http://catfact.ninja) - this is an API that returns random cat facts. This is a fun API to use to show how you can integrate any API that returns textual data into your LLM so that it can reason using that data.
- [Dall-E from  OpenAI](https://openai.com) - this is part of the OpenAI API that allows you to generate images from textual prompts. This is used to show how to pass data to a plugin, and then use that data to call an external API.

## Build SDKs for the APIs

To integrate the APIs into your LLM, you need to generate C# SDKs for them using liblab. To do this, follow these steps:

1. Ensure you have the liblab CLI installed and you are logged in. If you are using the included devcontainer, this is already installed for you. If not, refer to the [liblab CLI installation instructions](https://developers.liblab.com/cli/cli-installation/).
1. This repo includes liblab config files already set up to generate the SDKs for the relevant APIs. These are in the `sdks` folder.
    1. From the terminal, navigate to the `sdks/cat-facts-sdk` folder in this repo.
    1. Run `liblab build` to generate the Cat Facts SDK. The generated SDK will be in the `cat-facts-sdk/output/csharp` folder.
    1. From the terminal, navigate to the `sdks/world-time-sdk` folder in this repo.
    1. Run `liblab build` to generate the World Time SDK. The generated SDK will be in the `world-time-sdk/output/csharp` folder.
    1. From the terminal, navigate to the `sdks/open-ai` folder in this repo.
    1. Run `liblab build` to generate the OpenAI SDL. The generated SDK will be in the `open-ai/output/csharp` folder.

## Check out the integrations

These APIs are integrated using Semantic Kernel plugins, and can be found in the `src/LLMAndSDKDemo/Plugins` folder. These plugins implement functions marked with the `KernelFunction` attribute, which allows them to be called from the LLM, along with a `Description` attribute that describes the capabilities of the function to the LLM so it can reason as to what kernel functions to call.

```csharp
[KernelFunction]
[Description("Gets a cat fact.")]
public async Task<string> GetCatFact()
{
    var response = await _client.Facts.GetRandomFactAsync();
    return response.Fact;
}
```

For example, the `GetCatFact` function in the `CatFactPlugin` class calls the Cat Facts API and returns a random cat fact. The description `"Gets a cat fact."` tells the LLM what this function does, so if you were to use a prompt such as `Give me a fact about cats`, the LLM would know to call this function and include the result in the response.

The Dall-E plugin shows how you can pass data to a plugin.

```csharp
[KernelFunction]
[Description("Creates an image using a given prompt.")]
public async Task<string> CreateImage(
    [Description("The prompt to use to create an image")] string prompt
    )
{
}
```

The `prompt` parameter has a description that tells the orchestrator what the parameter is for. This allows the orchestrator to pass the data to the plugin, and the plugin can then use that data to call the OpenAI SDK and create an image.

## Configure the app

To run this app you will need to configure your OpenAI API key in the app settings.

1. In the `src/LLMAndSDKDemo` folder, copy the `appsettings.json.example` file to `appsettings.json`.
1. Open the `appsettings.json` file and replace `OpenAI/key` with your OpenAI API key.

    ```json
    {
        "OpenAI": {
            "Key": "", // Your OpenAI API key
            "ChatModel": "gpt-4",
            "ImageModel": "dall-e-3"
        }
    }
    ```

1. The app uses GPT-4 by default as the chat model, and Dall-E-3 for image generation. If you want to use another model, replace the relevant values with the model Ids of the models you want to use.

## Try the app out

The app is a simple .NET console app that allows you to interact with the LLM. The project is configured to point to the generated SDKs, so assuming you have these generated you should be able to build and run the app.

1. Navigate to the `src/LLMAndSDKDemo` folder in the terminal.
1. Run `dotnet run` to build and run the app.
1. Give the AI a prompt. If your prompt involves the current date and time or a cat fact, the AI will call the appropriate plugin to get the information, and you will see this written to the console. If your prompt involves an image, the AI will call the OpenAI API to generate an image and write this to the `images` folder.

    ```bash
    $ dotnet run
    I am an AI assistant who also knows the current day and time, and a load of cat facts!
    User > Tell me a fact about cats
    CatFactPlugin > Getting a cat fact from the Cat Facts API...
    Assistant > Here is a fun fact: Approximately 1/3 of cat owners think their pets are able to read their minds.
    User > What is the date
    DayPlugin > Getting the date and time from the World Time API...
    Assistant > The date is April 5, 2024.
    User > Create an image using a cat fact
    DallEPlugin > Creating an image using the prompt: A cat traveling at a top speed of approximately 31 mph over a short distance
    Assistant > Here is the image generated based on the cat fact: images/40f58c1b-b01d-4a05-9f0f-0a11974b1950.png
    ```

    The LLM uses the history of the conversation, so you can ask questions based on previous responses. For example, you could ask `What is the date tomorrow?` and the LLM would know to call the World Time API to get the date for tomorrow.

    ```bash
    User > what is the date tomorrow
    DayPlugin > Getting the date and time from the World Time API...
    Assistant > Tomorrow's date will be April 6, 2024.
    ```

## Extend the app

This is your opportunity to extend this app! Find APIs you are interested in, and use liblab to generate C# SDKs for them. Then integrate them into the app using Semantic Kernel plugins.
