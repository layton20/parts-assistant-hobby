using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;
using PartsAssistant.Langfuse;

const string ActivitySourceName = "PartsAssistant.TraceSmokeTest";

using var activitySource = new ActivitySource(ActivitySourceName);
// Register this source and send its spans to Langfuse.
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(ActivitySourceName)
    .AddLangfuseExporter(LangfuseOptions.FromEnvironment())
    .Build();

// Emit a sample span with input and output fields for the Langfuse UI.
using (var activity = activitySource.StartActivity("smoke-test"))
{
    activity?.SetTag("langfuse.observation.input", "Do you have front brake pads for a 2017 Halden Corvid 1.6 Petrol?");
    activity?.SetTag("langfuse.observation.output", "BP-1042-F");
}

// Flush queued spans before the process exits.
tracerProvider.ForceFlush();
Console.WriteLine("Trace sent. Check the Langfuse UI.");