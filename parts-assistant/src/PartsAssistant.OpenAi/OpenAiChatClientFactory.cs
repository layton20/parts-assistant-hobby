using Microsoft.Extensions.AI;
using OpenAI.Chat;

namespace PartsAssistant.OpenAi;

public static class OpenAiChatClientFactory
{
    public const string ApiKeyVariableName = "OPENAI_API_KEY";
    public const string ActivitySourceName = "PartsAssistant.OpenAi";

    // Check platform.openai.com for the current cheapest model before relying on this id.
    private const string ModelId = "gpt-5-nano";

    public static IChatClient CreateFromEnvironment()
    {
        var apiKey = Environment.GetEnvironmentVariable(ApiKeyVariableName)
            ?? throw new InvalidOperationException($"Set the {ApiKeyVariableName} environment variable.");

        return new ChatClient(ModelId, apiKey)
            .AsIChatClient()
            .AsBuilder()
            .UseOpenTelemetry(sourceName: ActivitySourceName, configure: client => client.EnableSensitiveData = true)
            .Build();
    }
}
