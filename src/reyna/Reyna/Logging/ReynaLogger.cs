
namespace Reyna
{
    public class ReynaLogger : IReynaLogger
    {
        private const uint LogError =   0;
        private const uint LogWarn =    1;
        private const uint LogDebug =   2;
        private const uint LogInfo =    3;
        private const uint LogVerbose = 4;

        public delegate void LogHandler(uint level, string format, params object[] args);

        private LogHandler logHandler;

        public ReynaLogger()
        {
            logHandler = null;
        }

        public void Initialise(LogHandler logger)
        {
            logHandler = logger;
        }

        public void Error(string msg, params object[] args)
        {
            if (logHandler != null)
                logHandler(LogError, msg, args);
        }

        public void Warn(string msg, params object[] args)
        {
            if (logHandler != null)
                logHandler(LogWarn, msg, args);
        }

        public void Debug(string msg, params object[] args)
        {
            if (logHandler != null)
                logHandler(LogDebug, msg, args);
        }

        public void Info(string msg, params object[] args)
        {
            if (logHandler != null)
                logHandler(LogInfo, msg, args);
        }
   
        public void Verbose(string msg, params object[] args)
        {
            if (logHandler != null)
                logHandler(LogVerbose, msg, args);
        }
    }
}
