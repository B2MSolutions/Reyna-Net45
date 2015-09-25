namespace Reyna.Interfaces
{
    public interface IWaitHandle
    {
        bool Set();

        bool WaitOne();

        bool Reset();
    }
}
