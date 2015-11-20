
namespace Reyna
{
    using Interfaces;

    public interface IReynaLogger
    {
        void Initialise(ILogDelegate logger);

        void Error(string msg, params object[] args);
        void Warn(string format, params object[] args);
        void Debug(string format, params object[] args);
        void Info(string msg, params object[] args);
        void Verbose(string format, params object[] args);
    } 
}
