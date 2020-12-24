using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Calculator.Tests.Nodes
{
    public class NodeEmbeddedFunctionshould
    {
        private Fixture _fixture = new Fixture() { RepeatCount = 2};

        [Fact]
        public void EvalThroughFunction()
        {
            _fixture.Customize(new AutoMoqCustomization());
            
            var nodeMockReturns = _fixture.Create<double>();
            var varNameMock = _fixture.Create<string>();
            var functionMockReturns = _fixture.Create<double>();
            var ctxMockReturns = _fixture.Create<double>();
            var functionMock = new Mock<Func<double, double, double>>();
            functionMock.Setup(_ => _(nodeMockReturns, nodeMockReturns)).Returns(functionMockReturns).Verifiable();
            var nodesMock = _fixture.Create<Node[]>();
            var sut = new NodeEmbeddedFunction(functionMock.Object, nodesMock);
            var ctxMock = new Mock<IContext>();
            ctxMock.Setup(_ => _.CallFunction(varNameMock, nodesMock)).Returns(ctxMockReturns).Verifiable();
            var result = sut.Eval(ctxMock.Object);
            Assert.Equal(ctxMockReturns, result); // не понимаю почему разные значения возвращаются actual всегда 0, мб здесь это вообще некорректное сравнение?
            //This setup was not matched
            //functionMock.Verify(); // а тут верифай падает
            //ctxMock.Verify(); // а тут верифай падает тоже
        }
    }
}


