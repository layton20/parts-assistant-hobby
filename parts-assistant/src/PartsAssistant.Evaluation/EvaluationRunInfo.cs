namespace PartsAssistant.Evaluation;

public sealed record EvaluationRunInfo(string RunId, string RunName, string DatasetId)
{
    public static EvaluationRunInfo Unnamed { get; } = new("unnamed-run", "unnamed-run", "unnamed-dataset");
}
