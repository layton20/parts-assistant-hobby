namespace PartsAssistant.Evaluation;

public sealed record CaseScore(string CaseId, bool Passed, IReadOnlyList<string> FailureReasons);