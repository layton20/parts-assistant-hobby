using System.Text;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

namespace PartsAssistant.Langfuse;

public static class LangfuseTracerProviderBuilderExtensions
{
    private const string TracesPath = "/api/public/otel/v1/traces";
    private const string IngestionVersionHeader = "x-langfuse-ingestion-version=4";

    public static TracerProviderBuilder AddLangfuseExporter(this TracerProviderBuilder builder, LangfuseOptions options) =>
        builder
            .AddSource(LangfuseExperimentObserver.ActivitySourceName)
            .AddProcessor(new LangfuseBaggageProcessor())
            .AddOtlpExporter(exporterOptions =>
            {
                exporterOptions.Endpoint = new Uri(new Uri(options.BaseUrl), TracesPath);
                exporterOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
                exporterOptions.Headers = $"Authorization=Basic {LangfuseAuth.CreateBasicCredentialsHeaderValue(options)},{IngestionVersionHeader}";
            });
}