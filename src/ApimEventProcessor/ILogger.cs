namespace ApimEventProcessor
{
    /// <summary>
    /// Abstracts away the logging messages
    /// </summary>
    public interface ILogger
    {
        void LogDebug(string message, params object[] parameters);
        void LogInfo(string message, params object[] parameters);
        void LogWarning(string message, params object[] parameters);
        void LogError(string message, params object[] parameters);
    }
}