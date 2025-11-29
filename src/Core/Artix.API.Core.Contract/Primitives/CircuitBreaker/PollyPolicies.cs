namespace Artix.API.Core.Contract.Primitives.CircuitBreaker;

using Polly;

public static class PollyPolicies
{
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => (int)r.StatusCode >= 500) // Retry only server errors
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retry =>
                    TimeSpan.FromMilliseconds(Math.Pow(2, retry) * 200), // exponential backoff
                onRetry: (outcome, timespan, retry, _) =>
                {
                    Console.WriteLine($"[Retry {retry}] Delaying {timespan.TotalMs()}ms due to: {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                });
    }

    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy()
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(
            TimeSpan.FromSeconds(5));
    }

    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(r => (int)r.StatusCode >= 500)
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(20),
                onBreak: (result, ts) =>
                {
                    Console.WriteLine($"Circuit BROKEN for {ts.TotalSeconds} sec");
                },
                onReset: () => Console.WriteLine("Circuit RESET"),
                onHalfOpen: () => Console.WriteLine("Circuit HALF-OPEN"));
    }

    private static double TotalMs(this TimeSpan ts) => ts.TotalMilliseconds;
}
