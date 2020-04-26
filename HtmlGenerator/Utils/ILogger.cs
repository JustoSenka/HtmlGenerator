namespace HtmlGenerator.Utils
{
    public interface ILogger
    {
        void LogError(string message);
        void LogWarning(string message);
        void LogMessage(string message);
    }
}
