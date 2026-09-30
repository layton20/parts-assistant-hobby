namespace PartsAssistant.Evaluation;

public sealed class NoOpEvaluationItemScope : IEvaluationItemScope
{
    public static NoOpEvaluationItemScope Instance { get; } = new();

    public Task CompleteAsync(CaseResult caseResult, CancellationToken cancellationToken) => Task.CompletedTask;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
