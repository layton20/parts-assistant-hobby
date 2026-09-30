namespace PartsAssistant.Evaluation;

public sealed class NoOpEvaluationRunObserver : IEvaluationRunObserver
{
    public static NoOpEvaluationRunObserver Instance { get; } = new();

    public IEvaluationItemScope BeginItem(EvaluationRunInfo runInfo, EvaluationCase evaluationCase) =>
        NoOpEvaluationItemScope.Instance;
}
