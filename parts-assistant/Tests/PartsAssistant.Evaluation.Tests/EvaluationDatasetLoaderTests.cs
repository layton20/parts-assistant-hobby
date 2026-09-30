using PartsAssistant.Assistant;
using PartsAssistant.Evaluation.Enums;

namespace PartsAssistant.Evaluation.Tests;

public sealed class EvaluationDatasetLoaderTests
{
    private const string DatasetDirectoryName = "Dataset";
    private const string DatasetFileName = "evaluation-cases.json";

    [Fact]
    public void LoadFromJson_UsesAssistantOutcome()
    {
        AssistantOutcome outcome = EvaluationDatasetLoader.LoadFromJson($"[{CreateCaseJson()}]").Single().ExpectedOutcome;

        Assert.Equal(AssistantOutcome.Answer, outcome);
    }

    [Fact]
    public void LoadFromFile_CommittedDataset_CoversEveryCategory()
    {
        var datasetPath = Path.Combine(AppContext.BaseDirectory, DatasetDirectoryName, DatasetFileName);

        var evaluationCases = EvaluationDatasetLoader.LoadFromFile(datasetPath);

        var uncoveredCategories = Enum.GetValues<EvaluationCategory>()
            .Except(evaluationCases.Select(evaluationCase => evaluationCase.Category));
        Assert.Empty(uncoveredCategories);
    }

    [Fact]
    public void LoadFromJson_WithDuplicateIds_Throws()
    {
        var json = $"[{CreateCaseJson()},{CreateCaseJson()}]";

        Assert.Throws<InvalidDataException>(() => EvaluationDatasetLoader.LoadFromJson(json));
    }

    [Fact]
    public void LoadFromJson_WithBlankQuestion_Throws()
    {
        var json = $"[{CreateCaseJson(question: " ")}]";

        Assert.Throws<InvalidDataException>(() => EvaluationDatasetLoader.LoadFromJson(json));
    }

    [Fact]
    public void LoadFromJson_AnswerWithoutRequiredParts_Throws()
    {
        var json = $"[{CreateCaseJson(expectedOutcome: "answer", requiredPartNumbersJson: "[]")}]";

        Assert.Throws<InvalidDataException>(() => EvaluationDatasetLoader.LoadFromJson(json));
    }

    [Fact]
    public void LoadFromJson_AbstainWithRequiredParts_Throws()
    {
        var json = $"[{CreateCaseJson(expectedOutcome: "abstain", requiredPartNumbersJson: """["BP-1"]""")}]";

        Assert.Throws<InvalidDataException>(() => EvaluationDatasetLoader.LoadFromJson(json));
    }

    [Fact]
    public void LoadFromJson_PartBothRequiredAndForbidden_Throws()
    {
        var json = $"[{CreateCaseJson(forbiddenPartNumbersJson: """["BP-1"]""")}]";

        Assert.Throws<InvalidDataException>(() => EvaluationDatasetLoader.LoadFromJson(json));
    }

    private static string CreateCaseJson(
        string id = "case-a",
        string question = "Do you have BP-1?",
        string expectedOutcome = "answer",
        string requiredPartNumbersJson = """["BP-1"]""",
        string forbiddenPartNumbersJson = "[]") =>
        $$"""
        {
          "id": "{{id}}",
          "question": "{{question}}",
          "category": "supersession",
          "expectedOutcome": "{{expectedOutcome}}",
          "requiredPartNumbers": {{requiredPartNumbersJson}},
          "forbiddenPartNumbers": {{forbiddenPartNumbersJson}}
        }
        """;
}