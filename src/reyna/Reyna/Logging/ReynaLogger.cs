
namespace Reyna
{
    using Interfaces;

    public class ReynaLogger : IReynaLogger
    {
        private const uint LogError =   0;
        private const uint LogWarn =    1;
        private const uint LogDebug =   2;
        private const uint LogInfo =    3;
        private const uint LogVerbose = 4;

        private ILogDelegate LogHandler;

        public ReynaLogger()
        {
            LogHandler = null;
        }

        public void Initialise(ILogDelegate logger)
        {
            LogHandler = logger;
        }

        public void Error(string msg, params object[] args)
        {
            if (LogHandler != null)
                LogHandler.Log(LogError, msg, args);
        }

        public void Warn(string msg, params object[] args)
        {
            if (LogHandler != null)
                LogHandler.Log(LogWarn, msg, args);
        }

        public void Debug(string msg, params object[] args)
        {
            if (LogHandler != null)
                LogHandler.Log(LogDebug, msg, args);
        }

        public void Info(string msg, params object[] args)
        {
            if (LogHandler != null)
                LogHandler.Log(LogInfo, msg, args);
        }
   
        public void Verbose(string msg, params object[] args)
        {
            if (LogHandler != null)
                LogHandler.Log(LogVerbose, msg, args);
        }
    }
}
