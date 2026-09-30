using System.Net.Http.Headers;
using System.Text;

namespace PartsAssistant.Langfuse;

public sealed class LangfuseScoreClient : IDisposable
{
    private const string ScoresPath = "/api/public/scores";
    private const string PassedScoreName = "passed";
    private const string BooleanDataType = "BOOLEAN";

    private readonly HttpClient httpClient;

    public LangfuseScoreClient(LangfuseOptions options)
    {
        httpClient = new HttpClient { BaseAddress = new Uri(options.BaseUrl) };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Basic",
            LangfuseAuth.CreateBasicCredentialsHeaderValue(options));
    }

    public async Task SendPassedScoreAsync(
        string traceId,
        string observationId,
        bool passed,
        string? comment,
        CancellationToken cancellationToken)
    {
        var request = new CreateScoreRequest(
            TraceId: traceId,
            ObservationId: observationId,
            Name: PassedScoreName,
            Value: passed ? 1 : 0,
            DataType: BooleanDataType,
            Comment: comment);

        using var content = new StringContent(LangfuseJson.Serialize(request), Encoding.UTF8, "application/json");
        using var response = await httpClient.PostAsync(ScoresPath, content, cancellationToken);
        
        response.EnsureSuccessStatusCode();
    }

    public void Dispose() => httpClient.Dispose();

    private sealed record CreateScoreRequest(
        string TraceId,
        string ObservationId,
        string Name,
        double Value,
        string DataType,
        string? Comment);
}
