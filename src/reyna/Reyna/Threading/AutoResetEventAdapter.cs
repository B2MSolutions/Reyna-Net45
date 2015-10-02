namespace Reyna
{
    using System.Threading;
    using Reyna.Interfaces;

    internal sealed class AutoResetEventAdapter : IAutoResetEventAdapter
    {
        public AutoResetEventAdapter()
        {
            this.Initialize(false);
        }

        public void Initialize(bool initialState)
        {
            this.AutoResetEvent = new AutoResetEvent(initialState);
        }

        private AutoResetEvent AutoResetEvent { get; set; }

        public bool Set()
        {
            return this.AutoResetEvent.Set();
        }

        public bool WaitOne()
        {
            return this.AutoResetEvent.WaitOne();
        }

        public bool Reset()
        {
            return this.AutoResetEvent.Reset();
        }
    }
}
