# Integrate liblab generated SDKs with your LLM using Semantic Kernel

This repo shows you how you can integrate your own data sources into an LLM powered app using auto-generated C# SDKs from [liblab](https://liblab.com), [Microsoft Semantic Kernel](https://learn.microsoft.com/semantic-kernel/), and [OpenAI](https://openai.com).

Semantic Kernel is a library that allows you use an LLM to call agents in your own code, using that data to augment the responses, or to run actions. This demo shows how you can quickly create a C# SDK for your APIs and integrate those SDKs into Semantic Kernel plugins, bringing your data into the LLM. 

## Prerequisites

To build and run this demo you will need:

- Visual Studio or Visual Studio Code
- Either .NET 8.0 installed locally, or docker installed so you can use the included devcontainer (recommended)
- An OpenAI API key
- A [liblab](https://liblab.com) account

    If you don't have a liblab account, you can create one [from liblab.com by selecting **Get Started**](https://liblab.com). You can sign up for free to get 15 SDK builds.

## Integrated APIs

This demo integrates two APIs:

- [World Time API](http://worldtimeapi.org) - this is an API that can get the current date and time for you based off your IP address. LLMs don't know the current date and time, so access to this information can be useful. Although this is overkill to use an API instead of just getting the current date and time from the system, it's a good example of how you can integrate any API into your LLM.
- [Cat Facts API](http://catfact.ninja) - this is an API that returns random cat facts. This is a fun API to use to show how you can integrate any API that returns textual data into your LLM so that it can reason using that data.

## Build SDKs for the APIs

To integrate the APIs into your LLM, you need to generate C# SDKs for them using liblab. To do this, follow these steps:

1. Ensure you have the liblab CLI installed and you are logged in. If you are using the included devcontainer, this is already installed for you. If not, refer to the [liblab CLI installation instructions](https://developers.liblab.com/cli/cli-installation/).
1. This repo includes liblab config files already set up to generate the SDKs for the World Time API and Cat Facts API.
    1. From the terminal, navigate to the `cat-facts-sdk` folder in this repo.
    1. Run `liblab build` to generate the Cat Facts SDK. The generated SDK will be in the `cat-facts-sdk/output/csharp` folder.
    1. From the terminal, navigate to the `world-time-sdk` folder in this repo.
    1. Run `liblab build` to generate the World Time SDK. The generated SDK will be in the `world-time-sdk/output/csharp` folder.

## Check out the integrations

These APIs are integrated using Semantic Kernel plugins, and can be found in the `src/DayCatFacts/Plugins` folder. These plugins implement functions marked with the `KernelFunction` attribute, which allows them to be called from the LLM, along with a `Description` attribute that describes the capabilities of the function to the LLM so it can reason as to what kernel functions to call.

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

## Configure the app

To run this app you will need to configure your OpenAI API key.