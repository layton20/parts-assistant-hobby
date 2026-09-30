using PartsAssistant.Assistant;
using PartsAssistant.Evaluation.Enums;

namespace PartsAssistant.Evaluation;

public sealed record EvaluationCase(string Id,
    string Question,
    EvaluationCategory Category,
    AssistantOutcome ExpectedOutcome,
    IReadOnlyList<string> RequiredPartNumbers,
    IReadOnlyList<string> ForbiddenPartNumbers);