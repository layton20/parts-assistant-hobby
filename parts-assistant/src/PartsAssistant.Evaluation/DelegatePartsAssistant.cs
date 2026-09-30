using PartsAssistant.Assistant;

namespace PartsAssistant.Evaluation;

public sealed class DelegatePartsAssistant(Func<string, AssistantResponse> respond) : IPartsAssistant
{
    public Task<AssistantResponse> AskAsync(string question, CancellationToken cancellationToken) =>
        Task.FromResult(respond(question));
}
