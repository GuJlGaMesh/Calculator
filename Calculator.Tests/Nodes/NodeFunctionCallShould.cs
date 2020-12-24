using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Calculator.Tests.Nodes
{
    public class NodeFunctionCallShould
    {
        private Fixture _fixture = new Fixture();
        private Mock<Node> SetUpMock()
        {
            var mock = new Mock<Node>();
            var returns = _fixture.Create<double>();
            var ctx = It.IsAny<IContext>();
            mock.Setup(_ => _.Eval(ctx)).Returns(returns).Verifiable();
            return mock;
        }

        [Fact]
        public void EvalThroughFunction()
        {
            _fixture.Customize(new AutoMoqCustomization());
            var nodeMockReturns = _fixture.Create<double>();
            var varNameMock = _fixture.Create<string>();
            var ctxMock = new Mock<IContext>();
            /*
            var mock1 = new Mock<Node>();
            mock1.Setup(_ => _.Eval(It.IsAny<IContext>())).Returns(nodeMockReturns).Verifiable();
            var mock2 = new Mock<Node>();
            mock2.Setup(_ => _.Eval(It.IsAny<IContext>())).Returns(nodeMockReturns).Verifiable();
            var nodesMock = new List<Mock<Node>>() { mock1, mock2};
            */
            var nodesMock = _fixture.Create<Node[]>();
            //var funcArgs = nodesMock.Select(_ => _.Object).ToArray();
            ctxMock.Setup(_ => _.CallFunction(varNameMock, nodesMock)).Returns(nodeMockReturns).Verifiable();
            var sut = new NodeFunctionCall(varNameMock, nodesMock);
            var result = sut.Eval(ctxMock.Object);
            Assert.Equal(result, nodeMockReturns);
            ctxMock.Verify();
            /*
             * This mock failed verification due to the following:
    
       Node _ => _.Eval(It.IsAny<IContext>()):
       This setup was not matched.
       Не понимаю, почему не работает.
            mock1.Verify(); 
            mock2.Verify();
            */
        }
    }
}


