namespace Artix.API.Utils.Http;

using System.Text.RegularExpressions;

public static class HttpResponseMessageExtensions
{
    /// <summary>
    /// Ensures success and provides a structured, sanitized exception if the request fails.
    /// </summary>
    public static async Task EnsureSuccessStatusCodeSafeAsync(this HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        var raw = await SafeReadContent(response);

        var error = BuildStructuredError(response, raw);

        throw new HttpIntegrationException(error);
    }

    private static async Task<string> SafeReadContent(HttpResponseMessage response)
    {
        if (response.Content == null)
            return string.Empty;

        try
        {
            return await response.Content.ReadAsStringAsync();
        }
        catch
        {
            return "<unreadable-content>";
        }
    }

    private static IntegrationError BuildStructuredError(HttpResponseMessage response, string raw)
    {
        var sanitized = Sanitize(raw);

        return new IntegrationError
        {
            StatusCode = (int)response.StatusCode,
            ReasonPhrase = response.ReasonPhrase ?? "",
            Url = response.RequestMessage?.RequestUri?.ToString() ?? "",
            RawContent = sanitized
        };
    }

    private static string Sanitize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";

        // Example: remove tokens / passwords
        text = Regex.Replace(text, "(?i)\"?(token|password|secret)\"?\\s*:\\s*\".*?\"", "$1: \"***\"");
        return text;
    }
}

/// <summary>
/// Structured error object for logging & tracing.
/// </summary>
public class IntegrationError
{
    public int StatusCode { get; set; }
    public string ReasonPhrase { get; set; }
    public string Url { get; set; }
    public string RawContent { get; set; }
}

/// <summary>
/// Custom integration-layer exception.
/// </summary>
public class HttpIntegrationException : Exception
{
    public IntegrationError Error { get; }

    public HttpIntegrationException(IntegrationError error)
        : base(BuildMessage(error))
    {
        Error = error;
    }

    private static string BuildMessage(IntegrationError e)
        => $"HTTP {e.StatusCode} - {e.ReasonPhrase}\nURL: {e.Url}\nContent: {e.RawContent}";
}
