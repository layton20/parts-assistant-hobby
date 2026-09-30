using Microsoft.Extensions.AI;
using PartsAssistant.Assistant;

namespace PartsAssistant.OpenAi;

public sealed class OpenAiPartsAssistant(IChatClient chatClient, string catalogueContext) : IPartsAssistant
{
    private const string SystemPromptTemplate = """
    You are a parts lookup assistant for an automotive aftermarket parts trade counter.
    Answer only using the catalogue below. Never invent a part number, price, or stock level
    that is not written there.

    If a part is discontinued, name its replacement. If that replacement is also discontinued,
    keep following the chain of replacements until you reach a part that is current, and
    recommend that current part, not an intermediate one that is itself discontinued.

    Diesel engines use glow plugs, not spark plugs, for ignition. If a customer asks for spark
    plugs for a diesel vehicle, treat this as a request for the vehicle's correct ignition part.

    If nothing in the catalogue matches the customer's vehicle and request, or you cannot be
    confident which single part is correct, set outcome to "abstain" and explain why in the
    message, with an empty partNumbers list. Otherwise set outcome to "answer" and list every
    part number you are recommending in partNumbers.

    Catalogue:
    {0}
    """;

    public async Task<AssistantResponse> AskAsync(string question, CancellationToken cancellationToken)
    {
        ChatMessage[] messages =
        [
            new(ChatRole.System, string.Format(SystemPromptTemplate, catalogueContext)),
            new(ChatRole.User, question)
        ];

        var response = await chatClient.GetResponseAsync<AssistantResponse>(
            messages,
            AssistantJsonOptions.Default,
            cancellationToken: cancellationToken);

        return response.Result;
    }
}