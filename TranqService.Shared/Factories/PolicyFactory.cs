namespace TranqService.Shared.Factories;

public class PolicyFactory : IPolicyFactory
{
    private readonly ILogger _logger;

    public PolicyFactory(ILogger logger)
    {
        _logger = logger;
    }

    public AsyncRetryPolicy GetRetryPolicy(string callerName, int maxRetries = 10, int delayMs = 2000)
    {
        return GetRetryPolicy<Exception>(callerName, maxRetries, delayMs);
    }

    public AsyncRetryPolicy GetRetryPolicy<TException>(string callerName, int maxRetries = 10, int delayMs = 2000)
        where TException : Exception
    {
        IEnumerable<TimeSpan> delay = Backoff.LinearBackoff(TimeSpan.FromMilliseconds(delayMs), maxRetries);
        return Policy
            .Handle<TException>()
            .WaitAndRetryAsync(delay, (ex, after, retries, context) =>
            {
                if (retries == maxRetries)
                {
                    string errMsg = "{callerName} failed after MAX RETRIES {retries} after {after} with {ex}";
                    _logger.Error(errMsg, callerName, retries, after, ex);
                    throw new Exception(string.Format(errMsg, callerName, retries, after, ex));
                }

                _logger.Debug("{callerName} failed a retry #{retries} with {exception}",
                    callerName, retries, ex.Message);
            });
    }
}
