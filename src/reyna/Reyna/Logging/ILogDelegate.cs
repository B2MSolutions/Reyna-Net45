namespace Reyna.Interfaces
{
    public interface ILogDelegate
    {
        void Log(uint logLevel, string msg, params object[] args);
    }
}
