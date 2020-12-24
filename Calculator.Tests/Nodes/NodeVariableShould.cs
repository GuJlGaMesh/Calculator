using AutoFixture;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Calculator.Tests.Nodes
{
    public class NodeVariableShould
    {
        private Fixture _fixture = new Fixture();
        [Fact]
        public void EvalThroughFunction()
        {
            var nodeMockReturns = _fixture.Create<double>();
            var varNameMock = _fixture.Create<string>();
            var ctxMock = new Mock<IContext>();
            ctxMock.Setup(_ => _.ResolveVariable(varNameMock)).Returns(nodeMockReturns).Verifiable();
            var sut = new NodeVariable(varNameMock);
            var result = sut.Eval(ctxMock.Object);
            Assert.IsType<double>(result);
            Assert.Equal(nodeMockReturns, result);
            ctxMock.Verify();
        }
    }
}
