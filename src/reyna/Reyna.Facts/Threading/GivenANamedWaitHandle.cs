namespace Reyna.Facts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Xunit;

    public class GivenANamedWaitHandle
    {
        public GivenANamedWaitHandle()
        {
            this.NamedWaitHandle = new NamedWaitHandle();
            this.NamedWaitHandle.Initialize(false, "NAME");
        }

        private NamedWaitHandle NamedWaitHandle { get; set; }

        [Fact]
        public void WhenCallingSetShouldNotThrow()
        {
            this.NamedWaitHandle.Set();
        }

        [Fact]
        public void WhenCallingResetShouldNotThrow()
        {
            this.NamedWaitHandle.Reset();
        }
    }
}
