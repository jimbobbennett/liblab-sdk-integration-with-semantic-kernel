using System.ComponentModel;
using Microsoft.SemanticKernel;
using OpenAi;
using OpenAi.Config;
using OpenAi.Models;
using static OpenAi.Models.CreateImageRequest;

namespace LLMAndSDKDemo.Plugins;

public class DallEPlugin
{
    private readonly OpenAiClient client;
    private readonly string imageModel;

    public DallEPlugin(string bearerToken, string imageModel)
    {
        var config = new OpenAiConfig
        {
            Environment = OpenAi.Http.Environment.Default,
            AccessToken = bearerToken
        };
        client = new OpenAiClient(config);
        this.imageModel = imageModel;

        // Create a folder to store the created images
        if (!Directory.Exists("images"))
        {
            Directory.CreateDirectory("images");
        }
    }

    [KernelFunction]
    [Description("Creates an image using a given prompt.")]
    public async Task<string> CreateImage(
        [Description("The prompt to use to create an image")] string prompt
        )
    {
        try
        {
            Console.WriteLine("DallEPlugin > Creating an image using the prompt: " + prompt);

            var model = imageModel switch
            {
                "dall-e-2" => Model23.DallE2,
                "dall-e-3" => Model23.DallE3,
                _ => Model23.DallE3
            };
            
            var createRequest = new CreateImageRequest(prompt)
            {
                Model = model,
                N = 1,
                Size = CreateImageRequestSize._1024x1024,
                ResponseFormat = CreateImageRequestResponseFormat.B64Json,
                Style_ = Style.Natural
            };
            var imageResponse = await client.Images.CreateImageAsync(createRequest);

            var imagePaths = new List<string>();

            foreach (var image in imageResponse.Data)
            {
                // Write the raw bytes to a file
                var imageBytes = Convert.FromBase64String(image.B64Json);
                // get a unique file name
                var fileName = Guid.NewGuid().ToString() + ".png";

                // Write the image to a file
                using var fileStream = new FileStream("images/" + fileName, FileMode.Create);
                fileStream.Write(imageBytes, 0, imageBytes.Length);

                imagePaths.Add("images/" + fileName);
            }

            string message;

            if (imagePaths.Count != 0)
            {
                message = "Images created: " + string.Join(", ", imagePaths);
            }
            else
            {
                message = "No images were created.";
            }

            Console.WriteLine("DallEPlugin > " + message);
            return message;
        }
        catch (Exception e)
        {
            Console.WriteLine("DallEPlugin > Error creating image: " + e.Message);
            return "DallEPlugin > Error creating image: " + e.Message;
        }
    }
}