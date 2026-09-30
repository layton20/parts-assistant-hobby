namespace PartsAssistant.Assistant;

public interface IPartsAssistant
{
    Task<AssistantResponse> AskAsync(string question, CancellationToken cancellationToken);
}