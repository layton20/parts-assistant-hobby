using PartsAssistant.Assistant;

namespace PartsAssistant.Evaluation;

public static class EvaluationCaseScorer
{
    public static CaseScore Score(EvaluationCase evaluationCase, AssistantResponse response)
    {
        var failureReasons = new List<string>();

        if (response.Outcome != evaluationCase.ExpectedOutcome)
        {
            failureReasons.Add(
                $"Expected outcome {evaluationCase.ExpectedOutcome} but the assistant chose {response.Outcome}.");
        }

        failureReasons.AddRange(evaluationCase.RequiredPartNumbers
            .Where(requiredPartNumber => !ContainsPartNumber(response.PartNumbers, requiredPartNumber))
            .Select(requiredPartNumber => $"Missing required part {requiredPartNumber}."));

        failureReasons.AddRange(evaluationCase.ForbiddenPartNumbers
            .Where(forbiddenPartNumber =>
                ContainsPartNumber(response.PartNumbers, forbiddenPartNumber)
                || (response.Outcome == AssistantOutcome.Answer && MessageMentionsPartNumber(response.Message, forbiddenPartNumber)))
            .Select(forbiddenPartNumber => $"Mentions forbidden part {forbiddenPartNumber}."));

        return new CaseScore(evaluationCase.Id, failureReasons.Count == 0, failureReasons);
    }

    private static bool ContainsPartNumber(IReadOnlyList<string> partNumbers, string partNumber) =>
        partNumbers.Contains(partNumber, StringComparer.OrdinalIgnoreCase);

    private static bool MessageMentionsPartNumber(string message, string partNumber) =>
        message.Contains(partNumber, StringComparison.OrdinalIgnoreCase);
}