using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace Calculator.Tests.Nodes
{
    public class NodeNumberShould
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(0.123456789)]
        [InlineData(100)]
        public void EvalAsIs(double expected)
        {
            var sut = new NodeNumber(expected);
            var actual = sut.Eval(null);
            Assert.Equal(expected, actual);
        }
    }

}
