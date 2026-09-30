using PartsAssistant.Assistant;

namespace PartsAssistant.Evaluation.Tests;

public sealed class EvaluationReportFormatterTests
{
    [Fact]
    public async Task Format_ListsEachFailedCaseWithItsReasons()
    {
        var cases = EvaluationDatasetLoader.LoadFromFile(
            Path.Combine(AppContext.BaseDirectory, "Dataset", "evaluation-cases.json"));
        var alwaysAbstains = new DelegatePartsAssistant(
            _ => new AssistantResponse(AssistantOutcome.Abstain, [], "I can't help with that."));
        var report = await new EvaluationRunner(alwaysAbstains).RunAsync(cases, CancellationToken.None);

        var text = EvaluationReportFormatter.Format(report);

        var firstFailedResult = report.FailedResults.First();
        Assert.Contains("Failures:", text);
        Assert.Contains(firstFailedResult.EvaluationCase.Id, text);
        Assert.Contains(firstFailedResult.Score.FailureReasons[0], text);
    }
}