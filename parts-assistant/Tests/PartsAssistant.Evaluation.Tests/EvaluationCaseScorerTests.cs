using PartsAssistant.Assistant;
using PartsAssistant.Evaluation.Enums;

namespace PartsAssistant.Evaluation.Tests;

public sealed class EvaluationCaseScorerTests
{
    private const string FrontPads = "BP-1042-F";
    private const string RearPads = "BP-1042-R";

    [Fact]
    public void Score_WhenAnswerHasRequiredPartAndNoForbiddenPart_Passes()
    {
        var evaluationCase = CreateCase(AssistantOutcome.Answer, required: [FrontPads], forbidden: [RearPads]);
        var response = CreateResponse(AssistantOutcome.Answer, partNumbers: [FrontPads]);

        var score = EvaluationCaseScorer.Score(evaluationCase, response);

        Assert.True(score.Passed);
        Assert.Empty(score.FailureReasons);
    }

    [Fact]
    public void Score_IgnoresCaseOfPartNumbers()
    {
        var evaluationCase = CreateCase(AssistantOutcome.Answer, required: [FrontPads], forbidden: []);
        var response = CreateResponse(AssistantOutcome.Answer, partNumbers: ["bp-1042-f"]);

        Assert.True(EvaluationCaseScorer.Score(evaluationCase, response).Passed);
    }

    [Fact]
    public void Score_WhenRequiredPartIsMissing_FailsAndNamesThePart()
    {
        var evaluationCase = CreateCase(AssistantOutcome.Answer, required: [FrontPads], forbidden: []);
        var response = CreateResponse(AssistantOutcome.Answer, partNumbers: [RearPads]);

        var score = EvaluationCaseScorer.Score(evaluationCase, response);

        Assert.False(score.Passed);
        Assert.Contains(score.FailureReasons, reason => reason.Contains(FrontPads));
    }

    [Fact]
    public void Score_WhenForbiddenPartIsRecommended_Fails()
    {
        var evaluationCase = CreateCase(AssistantOutcome.Answer, required: [FrontPads], forbidden: [RearPads]);
        var response = CreateResponse(AssistantOutcome.Answer, partNumbers: [FrontPads, RearPads]);

        var score = EvaluationCaseScorer.Score(evaluationCase, response);

        Assert.False(score.Passed);
        Assert.Contains(score.FailureReasons, reason => reason.Contains(RearPads));
    }

    [Fact]
    public void Score_WhenForbiddenPartAppearsOnlyInTheMessage_Fails()
    {
        var evaluationCase = CreateCase(AssistantOutcome.Answer, required: [FrontPads], forbidden: [RearPads]);
        var response = CreateResponse(
            AssistantOutcome.Answer,
            partNumbers: [FrontPads],
            message: $"{FrontPads} fits. The rear pads are {RearPads}.");

        Assert.False(EvaluationCaseScorer.Score(evaluationCase, response).Passed);
    }

    [Fact]
    public void Score_WhenAssistantAnswersButShouldHaveAbstained_Fails()
    {
        var evaluationCase = CreateCase(AssistantOutcome.Abstain, required: [], forbidden: ["BP-3300-F"]);
        var response = CreateResponse(AssistantOutcome.Answer, partNumbers: ["BP-3300-F"]);

        var score = EvaluationCaseScorer.Score(evaluationCase, response);

        Assert.False(score.Passed);
        Assert.Contains(score.FailureReasons, reason => reason.Contains("Expected outcome Abstain"));
    }

    [Fact]
    public void Score_WhenAssistantAbstainsAsExpected_Passes()
    {
        var evaluationCase = CreateCase(AssistantOutcome.Abstain, required: [], forbidden: ["BP-3300-F"]);
        var response = CreateResponse(AssistantOutcome.Abstain, partNumbers: [], message: "I can't find a match for that vehicle year.");

        Assert.True(EvaluationCaseScorer.Score(evaluationCase, response).Passed);
    }

    [Fact]
    public void Score_WhenAbstainingAndMessageMentionsForbiddenPartAsExplanation_Passes()
    {
        var evaluationCase = CreateCase(AssistantOutcome.Abstain, required: [], forbidden: ["BP-3300-F"]);
        var response = CreateResponse(
            AssistantOutcome.Abstain,
            partNumbers: [],
            message: "Closest is BP-3300-F, but that only fits 2020-2024, not this 2019 car.");

        Assert.True(EvaluationCaseScorer.Score(evaluationCase, response).Passed);
    }

    [Fact]
    public void Score_WhenAbstainingButForbiddenPartIsInTheStructuredList_StillFails()
    {
        var evaluationCase = CreateCase(AssistantOutcome.Abstain, required: [], forbidden: ["BP-3300-F"]);
        var response = CreateResponse(AssistantOutcome.Abstain, partNumbers: ["BP-3300-F"], message: "Abstaining.");

        Assert.False(EvaluationCaseScorer.Score(evaluationCase, response).Passed);
    }

    private static EvaluationCase CreateCase(
        AssistantOutcome expectedOutcome,
        string[] required,
        string[] forbidden) =>
        new(
            Id: "case-a",
            Question: "Do you have front brake pads?",
            Category: EvaluationCategory.NearIdenticalNumber,
            ExpectedOutcome: expectedOutcome,
            RequiredPartNumbers: required,
            ForbiddenPartNumbers: forbidden);

    private static AssistantResponse CreateResponse(
        AssistantOutcome outcome,
        string[] partNumbers,
        string message = "Here you go.") =>
        new(outcome, partNumbers, message);
}