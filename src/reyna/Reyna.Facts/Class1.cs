using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Reyna;

namespace Reyna.Facts
{
    
    public class Class1
    {
        [Fact]
        public void test1()
        {
            Reyna.Class1 t = new Reyna.Class1();
            Assert.True(t.TestThis());
        }
    }
}
