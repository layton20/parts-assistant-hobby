namespace PartsAssistant.Assistant;

public interface IResponseQualityJudge
{
    Task<GroundednessJudgement> ScoreGroundednessAsync(
        string question,
        AssistantResponse response,
        string groundingContext,
        CancellationToken cancellationToken
    );
}
