using System.Text.Json;
using PartsAssistant.Assistant;
using PartsAssistant.Evaluation.Enums;

namespace PartsAssistant.Evaluation;

public static class EvaluationDatasetLoader
{
    public static IReadOnlyList<EvaluationCase> LoadFromFile(string filePath) => LoadFromJson(File.ReadAllText(filePath));

    public static IReadOnlyList<EvaluationCase> LoadFromJson(string json)
    {
        var evaluationCases = JsonSerializer.Deserialize<List<EvaluationCase>>(json, EvaluationJsonOptions.Default)
            ?? throw new InvalidDataException("The evaluation dataset is empty.");

        EnsureNoDuplicateIds(evaluationCases);
        evaluationCases.ForEach(EnsureCaseIsConsistent);

        return evaluationCases;
    }

    private static void EnsureNoDuplicateIds(IEnumerable<EvaluationCase> evaluationCases)
    {
        var duplicateGroup = evaluationCases
            .GroupBy(evaluationCase => evaluationCase.Id)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicateGroup is not null)
        {
            throw new InvalidDataException($"Duplicate evaluation case id: {duplicateGroup.Key}.");
        }
    }

    private static void EnsureCaseIsConsistent(EvaluationCase evaluationCase)
    {
        if (string.IsNullOrWhiteSpace(evaluationCase.Id) || string.IsNullOrWhiteSpace(evaluationCase.Question))
        {
            throw new InvalidDataException("Every evaluation case needs an id and a question.");
        }

        var hasRequiredParts = evaluationCase.RequiredPartNumbers.Count > 0;

        if (evaluationCase.ExpectedOutcome == AssistantOutcome.Answer && !hasRequiredParts)
        {
            throw new InvalidDataException($"Case {evaluationCase.Id} expects an answer but requires no part numbers.");
        }

        if (evaluationCase.ExpectedOutcome == AssistantOutcome.Abstain && hasRequiredParts)
        {
            throw new InvalidDataException($"Case {evaluationCase.Id} expects an abstention but requires part numbers.");
        }

        if (evaluationCase.RequiredPartNumbers.Intersect(evaluationCase.ForbiddenPartNumbers).Any())
        {
            throw new InvalidDataException($"Case {evaluationCase.Id} both requires and forbids the same part number.");
        }
    }
}