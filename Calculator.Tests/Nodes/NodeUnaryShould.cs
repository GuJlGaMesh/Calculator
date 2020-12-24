using AutoFixture;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
namespace Calculator.Tests.Nodes
{

    public class NodeUnaryShould
    {
        private Fixture _fixture = new Fixture();
        [Fact]
        public void EvalThroughFunction()
        {
            
        var nodeMockReturns = _fixture.Create<double>(); // можно использовать Random вместо AutoFixture. 
                                                        // AutoFixture позволяет создавать экземпляры классов, содержащие случайные данные, 
                                                        // но при этом можно вмешиваться в этот процесс и производить более тонкую настройку

        var nodeMock = new Mock<Node>(); // библиотека Moq. Служит для подмены реальных зависимостей.
                                         // в unit тестах проверяется небольшая часть функциональности объекта. 
                                         // Зачастую объект может вести себя по-разному в зависимости от своего состояния и состояния своих зависимостей.
                                         // Чтобы имитировать то состояние зависимостей, при котором необходимо протестировать sut (system under test), можно использовать Moq.

        // дословно: настраиваем мок так, что при вызове Eval с любым значение передаваемого значения, будет возвращено значение из nodeMockReurns.
        // Verifiable - говорит, что необходимо проверить был ли вызван этот метод именно с этими аргументами, которые настроены - в этом случае любое значение аргумента.
        nodeMock.Setup(_ => _.Eval(It.IsAny<IContext>())).Returns(nodeMockReturns).Verifiable();

        var functionMockReturns = _fixture.Create<double>();
        var functionMock = new Mock<Func<double, double>>();
        // настраиваем Mock функции - при вызове функции с аргументом nodeMockReurns, возвращаем functionMockReturns, также помечаем Verifiable.
        functionMock.Setup(_ => _(nodeMockReturns)).Returns(functionMockReturns).Verifiable();

        var sut = new NodeUnary(nodeMock.Object, functionMock.Object);

        var result = sut.Eval(null); // знаем, что IContext игнорируется - порицаем

        Assert.Equal(functionMockReturns, result); // ждем что результат будет равен, тому что вернет функция. Первым передается ожидаемое значение, вторым проверяемое.

        nodeMock.Verify(); // проверка всех Verifiable методов на nodeMock
        functionMock.Verify(); // проверка всех Verifiable методов на functionMock
                               // можно вместо отдельных вызовов проверить сразу все моки Mock.Verify(nodeMock, functionMock);
        }
    }
}
