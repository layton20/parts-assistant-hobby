using PartsAssistant.Assistant;
using PartsAssistant.Evaluation.Enums;

namespace PartsAssistant.Evaluation.Tests;

public sealed class EvaluationRunnerTests
{
    private static readonly IReadOnlyList<EvaluationCase> Cases =
    EvaluationDatasetLoader.LoadFromFile(Path.Combine(AppContext.BaseDirectory, "Dataset", "evaluation-cases.json"));

    [Fact]
    public async Task RunAsync_WithAssistantThatMatchesEveryLabel_PassesEveryCase()
    {
        var responsesByQuestion = Cases.ToDictionary(
            evaluationCase => evaluationCase.Question,
            evaluationCase => new AssistantResponse(
                evaluationCase.ExpectedOutcome,
                evaluationCase.RequiredPartNumbers,
                "Here is what I found."));
        var runner = new EvaluationRunner(new DelegatePartsAssistant(question => responsesByQuestion[question]));

        var report = await runner.RunAsync(Cases, CancellationToken.None);

        Assert.Equal(Cases.Count, report.PassedCount);
    }

    [Fact]
    public async Task RunAsync_WithAssistantThatAlwaysAbstains_PassesOnlyTheAbstainCases()
    {
        var runner = new EvaluationRunner(CreateAlwaysAbstainingAssistant());

        var report = await runner.RunAsync(Cases, CancellationToken.None);

        var abstainCaseCount = Cases.Count(evaluationCase => evaluationCase.ExpectedOutcome == AssistantOutcome.Abstain);
        Assert.Equal(abstainCaseCount, report.PassedCount);
    }

    [Fact]
    public async Task RunAsync_ReportsPassRateForEachCategory()
    {
        var runner = new EvaluationRunner(CreateAlwaysAbstainingAssistant());

        var report = await runner.RunAsync(Cases, CancellationToken.None);

        Assert.Equal(1.0, report.PassRateByCategory[EvaluationCategory.Unanswerable]);
        Assert.Equal(0.0, report.PassRateByCategory[EvaluationCategory.Supersession]);
    }

    [Fact]
    public async Task RunAsync_WithNoCases_Throws()
    {
        var runner = new EvaluationRunner(CreateAlwaysAbstainingAssistant());

        await Assert.ThrowsAsync<ArgumentException>(() => runner.RunAsync([], CancellationToken.None));
    }

    private static DelegatePartsAssistant CreateAlwaysAbstainingAssistant() =>
        new(_ => new AssistantResponse(AssistantOutcome.Abstain, [], "I can't help with that."));
}