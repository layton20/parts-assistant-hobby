using System.Diagnostics;
using OpenTelemetry;
using PartsAssistant.Evaluation;

namespace PartsAssistant.Langfuse;

internal sealed class LangfuseExperimentItemScope(
    Activity? rootActivity,
    Baggage previousBaggage,
    LangfuseScoreClient scoreClient) : IEvaluationItemScope
{
    private const string FailureReasonSeparator = "; ";

    private bool isCompleted;

    public async Task CompleteAsync(CaseResult result, CancellationToken cancellationToken)
    {
        rootActivity?.SetTag(LangfuseAttributes.ObservationOutput, LangfuseJson.Serialize(result.Response));
        isCompleted = true;

        if (rootActivity is null)
        {
            return;
        }

        var comment = result.Score.Passed ? null : string.Join(FailureReasonSeparator, result.Score.FailureReasons);

        await scoreClient.SendPassedScoreAsync(
            traceId: rootActivity.TraceId.ToHexString(),
            observationId: rootActivity.SpanId.ToHexString(),
            passed: result.Score.Passed,
            comment: comment,
            cancellationToken);
    }

    public ValueTask DisposeAsync()
    {
        if (!isCompleted)
        {
            rootActivity?.SetStatus(ActivityStatusCode.Error);
        }

        rootActivity?.Dispose();
        Baggage.Current = previousBaggage;
        return ValueTask.CompletedTask;
    }
}
