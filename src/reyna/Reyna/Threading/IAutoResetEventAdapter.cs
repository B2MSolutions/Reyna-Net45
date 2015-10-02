namespace Reyna.Interfaces
{
    interface IAutoResetEventAdapter : IWaitHandle
    {
        void Initialize(bool initialState);
    }
}
