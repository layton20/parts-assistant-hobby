namespace PartsAssistant.Langfuse;

public sealed record LangfuseOptions(string BaseUrl, string PublicKey, string SecretKey)
{
    public const string BaseUrlVariableName = "LANGFUSE_BASE_URL";
    public const string PublicKeyVariableName = "LANGFUSE_PUBLIC_KEY";
    public const string SecretKeyVariableName = "LANGFUSE_SECRET_KEY";

    private const string LocalBaseUrl = "http://localhost:3000";

    public static LangfuseOptions FromEnvironment() => new(
        Environment.GetEnvironmentVariable(BaseUrlVariableName) ?? LocalBaseUrl,
        ReadRequiredVariable(PublicKeyVariableName),
        ReadRequiredVariable(SecretKeyVariableName));

    public override string ToString() => $"LangfuseOptions {{ BaseUrl = {BaseUrl} }}";

    private static string ReadRequiredVariable(string variableName) =>
        Environment.GetEnvironmentVariable(variableName)
        ?? throw new InvalidOperationException($"Set the {variableName} environment variable.");
}