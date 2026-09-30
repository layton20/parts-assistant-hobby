using System.Diagnostics;
using OpenTelemetry;

namespace PartsAssistant.Langfuse;

internal sealed class LangfuseBaggageProcessor : BaseProcessor<Activity>
{
    public override void OnStart(Activity activity)
    {
        foreach (var (name, value) in Baggage.Current.GetBaggage())
        {
            if (name.StartsWith(LangfuseAttributes.Prefix, StringComparison.Ordinal))
            {
                activity.SetTag(name, value);
            }
        }
    }
}
