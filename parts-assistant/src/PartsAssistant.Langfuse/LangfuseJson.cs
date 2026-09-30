using System.Text.Json;
using System.Text.Json.Serialization;

namespace PartsAssistant.Langfuse;

internal static class LangfuseJson
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };

    public static string Serialize<TValue>(TValue value) => JsonSerializer.Serialize(value, Options);
}