namespace Reyna.Interfaces
{
    public interface ILoggerInterface
    {
        void LogDelegate(uint logLevel, string msg, params object[] args);
    }
}
