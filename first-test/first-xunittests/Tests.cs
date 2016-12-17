using System;
using Xunit;
using FirstClassLibrary;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void Test1() 
        {
            Assert.True(true);
            var str = Str.Upper("orz");
            Assert.Equal("ORZ",str);
            
        }
    }
}
