using System.Diagnostics;
using OpenTelemetry;
using PartsAssistant.Evaluation;

namespace PartsAssistant.Langfuse;

public sealed class LangfuseExperimentObserver(LangfuseScoreClient scoreClient) : IEvaluationRunObserver
{
    public const string ActivitySourceName = "PartsAssistant.Evaluation";
    private const string ExperimentEnvironment = "experiment";
    private const string ItemActivityName = "experiment-item";

    private static readonly ActivitySource ItemActivitySource = new(ActivitySourceName);

    public IEvaluationItemScope BeginItem(EvaluationRunInfo runInfo, EvaluationCase evaluationCase)
    {
        var previousBaggage = Baggage.Current;

        var experimentContext = new Dictionary<string, string>
        {
            [LangfuseAttributes.ExperimentId] = runInfo.RunId,
            [LangfuseAttributes.ExperimentName] = runInfo.RunName,
            [LangfuseAttributes.ExperimentDatasetId] = runInfo.DatasetId,
            [LangfuseAttributes.Environment] = ExperimentEnvironment
        };
        Baggage.Current = Baggage.Create(experimentContext);

        var rootActivity = ItemActivitySource.StartActivity(ItemActivityName, ActivityKind.Internal, parentContext: default);
        if (rootActivity is not null)
        {
            var rootObservationId = rootActivity.SpanId.ToHexString();

            rootActivity.SetTag(LangfuseAttributes.ItemId, evaluationCase.Id);
            rootActivity.SetTag(LangfuseAttributes.ItemRootObservationId, rootObservationId);
            rootActivity.SetTag(LangfuseAttributes.ObservationInput, evaluationCase.Question);
            rootActivity.SetTag(LangfuseAttributes.ItemExpectedOutput, LangfuseJson.Serialize(new
            {
                Outcome = evaluationCase.ExpectedOutcome,
                evaluationCase.RequiredPartNumbers,
                evaluationCase.ForbiddenPartNumbers
            }));

            var itemContext = new Dictionary<string, string>(experimentContext)
            {
                [LangfuseAttributes.ItemId] = evaluationCase.Id,
                [LangfuseAttributes.ItemRootObservationId] = rootObservationId
            };
            Baggage.Current = Baggage.Create(itemContext);
        }

        return new LangfuseExperimentItemScope(rootActivity, previousBaggage, scoreClient);
    }
}
