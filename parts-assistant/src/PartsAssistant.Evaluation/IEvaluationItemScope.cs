namespace PartsAssistant.Evaluation;

public interface IEvaluationItemScope : IAsyncDisposable
{
    Task CompleteAsync(CaseResult caseResult, CancellationToken cancellationToken);
}
