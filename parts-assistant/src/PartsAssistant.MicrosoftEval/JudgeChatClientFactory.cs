using Microsoft.Extensions.AI;
using OpenAI.Chat;

namespace PartsAssistant.MicrosoftEval;

public static class JudgeChatClientFactory
{
    private const string ApiKeyVariableName = "OPENAI_API_KEY";

    private const string JudgeModelId = "gpt-4.1-mini";

    public static IChatClient CreateFromEnvironment()
    {
        var apiKey = Environment.GetEnvironmentVariable(ApiKeyVariableName)
            ?? throw new InvalidOperationException($"Set the {ApiKeyVariableName} environment variable.");

        return new ChatClient(JudgeModelId, apiKey).AsIChatClient();
    }
}
