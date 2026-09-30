using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;
using PartsAssistant.Assistant;

namespace PartsAssistant.MicrosoftEval;

public sealed class GroundednessResponseQualityJudge(IChatClient judgeChatClient) : IResponseQualityJudge
{
    private readonly ChatConfiguration judgeConfiguration = new(judgeChatClient);
    private readonly GroundednessEvaluator evaluator = new();

    public async Task<GroundednessJudgement> ScoreGroundednessAsync(
        string question,
        AssistantResponse response,
        string groundingContext,
        CancellationToken cancellationToken)
    {
        ChatMessage[] messages = [new(ChatRole.User, question)];
        var chatResponse = new ChatResponse(new ChatMessage(ChatRole.Assistant, response.Message));
        var groundingEvaluatorContext = new GroundednessEvaluatorContext(groundingContext);

        var result = await evaluator.EvaluateAsync(
            messages,
            chatResponse,
            judgeConfiguration,
            [groundingEvaluatorContext],
            cancellationToken);

        var metric = result.Get<NumericMetric>(GroundednessEvaluator.GroundednessMetricName);

        return new GroundednessJudgement(metric.Value ?? 0, metric.Reason ?? string.Empty);
    }
}
