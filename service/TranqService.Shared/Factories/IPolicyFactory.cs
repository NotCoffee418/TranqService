namespace TranqService.Shared.Factories
{
    public interface IPolicyFactory
    {
        AsyncRetryPolicy GetRetryPolicy(string callerName, int maxRetries = 10, int delayMs = 2000);
        AsyncRetryPolicy GetRetryPolicy<TException>(string callerName, int maxRetries = 10, int delayMs = 2000) where TException : Exception;
    }
}