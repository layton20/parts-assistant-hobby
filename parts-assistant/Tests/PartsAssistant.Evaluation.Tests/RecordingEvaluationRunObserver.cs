namespace PartsAssistant.Evaluation.Tests;

internal sealed class RecordingEvaluationRunObserver : IEvaluationRunObserver
{
    public List<string> BegunCaseIds { get; } = [];
    public List<string> CompletedCaseIds { get; } = [];
    public int DisposedScopeCount { get; private set; }

    public IEvaluationItemScope BeginItem(EvaluationRunInfo runInfo, EvaluationCase evaluationCase)
    {
        BegunCaseIds.Add(evaluationCase.Id);
        return new RecordingScope(this);
    }

    private sealed class RecordingScope(RecordingEvaluationRunObserver owner) : IEvaluationItemScope
    {
        public Task CompleteAsync(CaseResult result, CancellationToken cancellationToken)
        {
            owner.CompletedCaseIds.Add(result.EvaluationCase.Id);
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            owner.DisposedScopeCount++;
            return ValueTask.CompletedTask;
        }
    }
}
