using PartsAssistant.Assistant;

namespace PartsAssistant.Evaluation;

public sealed class EvaluationRunner(IPartsAssistant assistant, IEvaluationRunObserver observer)
{
    public EvaluationRunner(IPartsAssistant assistant)
        : this(assistant, new NoOpEvaluationRunObserver())
    {
    }

    public Task<EvaluationReport> RunAsync(
        IReadOnlyList<EvaluationCase> evaluationCases,
        CancellationToken cancellationToken) =>
        RunAsync(evaluationCases, EvaluationRunInfo.Unnamed, cancellationToken);

    public async Task<EvaluationReport> RunAsync(
        IReadOnlyList<EvaluationCase> evaluationCases,
        EvaluationRunInfo runInfo,
        CancellationToken cancellationToken)
    {
        if (evaluationCases.Count == 0)
        {
            throw new ArgumentException("At least one evaluation case is required.", nameof(evaluationCases));
        }

        var results = new List<CaseResult>();

        foreach (var evaluationCase in evaluationCases)
        {
            await using var itemScope = observer.BeginItem(runInfo, evaluationCase);

            var response = await assistant.AskAsync(evaluationCase.Question, cancellationToken);
            var score = EvaluationCaseScorer.Score(evaluationCase, response);
            var result = new CaseResult(evaluationCase, response, score);

            await itemScope.CompleteAsync(result, cancellationToken);
            results.Add(result);
        }

        return new EvaluationReport(results);
    }
}