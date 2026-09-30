using System.Text;

namespace PartsAssistant.Langfuse;

internal static class LangfuseAuth
{
    public static string CreateBasicCredentialsHeaderValue(LangfuseOptions options) =>
        Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.PublicKey}:{options.SecretKey}"));
}