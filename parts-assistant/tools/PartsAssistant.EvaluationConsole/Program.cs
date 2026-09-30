using OpenTelemetry;
using OpenTelemetry.Trace;
using PartsAssistant.Assistant;
using PartsAssistant.Domain;
using PartsAssistant.Evaluation;
using PartsAssistant.Langfuse;
using PartsAssistant.MicrosoftEval;
using PartsAssistant.OpenAi;

const string DatasetId = "parts-assistant-eval";

// Load the fixed set of questions/expected answers this run will be scored against.
var datasetPath = Path.Combine(AppContext.BaseDirectory, "Dataset", "evaluation-cases.json");
var evaluationCases = EvaluationDatasetLoader.LoadFromFile(datasetPath);

// Load the parts/vehicles/fitments fixture and render it into the text block the
// OpenAI assistant gets as its catalogue context.
var cataloguePath = Path.Combine(AppContext.BaseDirectory, "Dataset", "catalogue-fixture.json");
var catalogueFixture = CatalogueFixtureLoader.LoadFromFile(cataloguePath);
var catalogueContext = CatalogueContextFormatter.Format(catalogueFixture.Parts, catalogueFixture.Vehicles, catalogueFixture.Fitments);
var partCatalogue = new InMemoryPartCatalogue(catalogueFixture.Parts);

// Wire up tracing and scoring so every run shows up as a Langfuse experiment.
var langfuseOptions = LangfuseOptions.FromEnvironment();

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(OpenAiChatClientFactory.ActivitySourceName)
    .AddLangfuseExporter(langfuseOptions)
    .Build();

using var scoreClient = new LangfuseScoreClient(langfuseOptions);
var observer = new LangfuseExperimentObserver(scoreClient);

// Real OpenAI client used by the assistant under test.
using var openAiChatClient = OpenAiChatClientFactory.CreateFromEnvironment();

var qualityJudge = new GroundednessResponseQualityJudge(JudgeChatClientFactory.CreateFromEnvironment());

// Canned "correct" answer for each question, used by the perfect-fake assistant below
// as a baseline that should always pass.
var perfectResponsesByQuestion = evaluationCases.ToDictionary(
    evaluationCase => evaluationCase.Question,
    evaluationCase => new AssistantResponse(
        evaluationCase.ExpectedOutcome,
        evaluationCase.RequiredPartNumbers,
        "Here is what I found."));

// The assistants to evaluate: two fakes as sanity-check baselines (always right,
// always abstains), plus the real OpenAI-backed assistant being tested.
var assistantsByName = new Dictionary<string, IPartsAssistant>
{
    // ["perfect-fake"] = new DelegatePartsAssistant(question => perfectResponsesByQuestion[question]),
    // ["always-abstains-fake"] = new DelegatePartsAssistant(
    //     _ => new AssistantResponse(AssistantOutcome.Abstain, [], "I can't help with that.")),
    ["openai-gpt5-nano"] = new ValidatingPartsAssistant(
        new OpenAiPartsAssistant(openAiChatClient, catalogueContext),
        partCatalogue)
};

// Run the evaluation dataset against every assistant and print a report for each.
foreach (var (assistantName, assistant) in assistantsByName)
{
    var runInfo = new EvaluationRunInfo(
        RunId: Guid.NewGuid().ToString(),
        RunName: $"{assistantName}-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
        DatasetId: DatasetId);

    var runner = new EvaluationRunner(assistant, observer);
    var report = await runner.RunAsync(evaluationCases, runInfo, CancellationToken.None);

    Console.WriteLine($"=== {runInfo.RunName}");
    Console.WriteLine(EvaluationReportFormatter.Format(report));

    if (assistantName == "openai-gpt5-nano")
    {
        Console.WriteLine("Groundedness judgements:");
        foreach (var result in report.Results)
        {
            var judgement = await qualityJudge.ScoreGroundednessAsync(
                result.EvaluationCase.Question, result.Response, catalogueContext, CancellationToken.None);
            Console.WriteLine($"  {result.EvaluationCase.Id}: {judgement.Score:F1}/5 - {judgement.Reasoning}");
        }
    }

    Console.WriteLine();
}

// Flush buffered spans/scores so they're visible in Langfuse before the process exits.
tracerProvider.ForceFlush();
Console.WriteLine("Runs sent. Check Experiments in Langfuse.");