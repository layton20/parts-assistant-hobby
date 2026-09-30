namespace PartsAssistant.Assistant;

public sealed record AssistantResponse(
    AssistantOutcome Outcome,
    IReadOnlyList<string> PartNumbers,
    string Message);