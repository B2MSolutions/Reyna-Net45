
namespace Reyna
{
    public interface IReynaLogger
    {
        void Initialise(ReynaLogger.LogHandler logger);

        void Error(string msg, params object[] args);
        void Warn(string format, params object[] args);
        void Debug(string format, params object[] args);
        void Info(string msg, params object[] args);
        void Verbose(string format, params object[] args);
    } 
}
