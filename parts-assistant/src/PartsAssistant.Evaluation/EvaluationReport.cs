using PartsAssistant.Evaluation.Enums;

namespace PartsAssistant.Evaluation;

public sealed record EvaluationReport(IReadOnlyList<CaseResult> Results)
{
    public int PassedCount => Results.Count(result => result.Score.Passed);

    public double PassRate => (double)PassedCount / Results.Count;

    public IEnumerable<CaseResult> FailedResults => Results.Where(result => !result.Score.Passed);

    public IReadOnlyDictionary<EvaluationCategory, double> PassRateByCategory => Results
        .GroupBy(result => result.EvaluationCase.Category)
        .ToDictionary(
            group => group.Key,
            group => (double)group.Count(result => result.Score.Passed) / group.Count());
}