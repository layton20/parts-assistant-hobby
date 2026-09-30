using PartsAssistant.Assistant;
using PartsAssistant.Evaluation.Enums;

namespace PartsAssistant.Evaluation.Tests;

public sealed class EvaluationRunnerObserverTests
{
    private static readonly EvaluationRunInfo RunInfo = new("run-id", "test-run", "test-dataset");

    private static readonly IReadOnlyList<EvaluationCase> Cases =
        EvaluationDatasetLoader.LoadFromFile(Path.Combine(AppContext.BaseDirectory, "Dataset", "evaluation-cases.json"));

    [Fact]
    public async Task RunAsync_OpensCompletesAndDisposesOneScopePerCase()
    {
        var observer = new RecordingEvaluationRunObserver();
        var assistant = new DelegatePartsAssistant(
            _ => new AssistantResponse(AssistantOutcome.Abstain, [], "I can't help with that."));
        var runner = new EvaluationRunner(assistant, observer);

        await runner.RunAsync(Cases, RunInfo, CancellationToken.None);

        var caseIds = Cases.Select(evaluationCase => evaluationCase.Id).ToList();
        Assert.Equal(caseIds, observer.BegunCaseIds);
        Assert.Equal(caseIds, observer.CompletedCaseIds);
        Assert.Equal(Cases.Count, observer.DisposedScopeCount);
    }

    [Fact]
    public async Task RunAsync_WhenAssistantThrows_DisposesTheScopeWithoutCompletingIt()
    {
        var observer = new RecordingEvaluationRunObserver();
        var throwingAssistant = new DelegatePartsAssistant(_ => throw new InvalidOperationException("The assistant failed."));
        var runner = new EvaluationRunner(throwingAssistant, observer);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => runner.RunAsync(Cases, RunInfo, CancellationToken.None));

        Assert.Single(observer.BegunCaseIds);
        Assert.Empty(observer.CompletedCaseIds);
        Assert.Equal(1, observer.DisposedScopeCount);
    }
}
