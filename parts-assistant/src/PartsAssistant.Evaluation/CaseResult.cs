using PartsAssistant.Assistant;

namespace PartsAssistant.Evaluation;

public sealed record CaseResult(EvaluationCase EvaluationCase, AssistantResponse Response, CaseScore Score);