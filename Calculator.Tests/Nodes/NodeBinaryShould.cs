using AutoFixture;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace Calculator.Tests.Nodes
{

    public class NodeBinaryShould
    {
        private Fixture _fixture = new Fixture();
        [Fact]
        public void EvalThroughFunction()
        {
            
        var nodeMockReturns = _fixture.Create<double>(); 
        var nodeMockLeft = new Mock<Node>(); 
        var nodeMockRight = new Mock<Node>();                               
        nodeMockLeft.Setup(_ => _.Eval(It.IsAny<IContext>())).Returns(nodeMockReturns).Verifiable();
        nodeMockRight.Setup(_ => _.Eval(It.IsAny<IContext>())).Returns(nodeMockReturns).Verifiable();

            var functionMockReturns = _fixture.Create<double>();
        var functionMock = new Mock<Func<double, double, double>>();
            // настраиваем Mock функции - при вызове функции с аргументом nodeMockReurns, возвращаем functionMockReturns, также помечаем Verifiable.
        functionMock.Setup(_ => _(nodeMockReturns, nodeMockReturns)).Returns(functionMockReturns).Verifiable();

        var sut = new NodeBinary(nodeMockLeft.Object, nodeMockRight.Object, functionMock.Object);

        var result = sut.Eval(null); // знаем, что IContext игнорируется - порицаем

        Assert.Equal(functionMockReturns, result); // ждем что результат будет равен, тому что вернет функция. Первым передается ожидаемое значение, вторым проверяемое.
        Mock.Verify(nodeMockLeft, nodeMockRight, functionMock);

        }
    }
}
