using System;
using System.IO;
using Xunit;


namespace Calculator.Tests
{
    public class Parser
    {
        [Theory]
        [InlineData("a=5 c =3 a+c", "8")]
        [InlineData("f(x) = x/2; g(x) = x+1; f(2)+g(2)", "4")]
        [InlineData(" a = 3 f(x) = x/2; g(x) = x+1; f(2)+a*g(2)", "10")]
        public void InlineInStrContext(string str, string expected)
        {
            var ctx = new Context();
            Assert.Equal(Calculator.Parser.Parse(str).Eval(ctx).ToString(), expected); // программно числа возвращаются с запятой, а анализируются с точкой, в ассертах надо 
            // тип дабл указывать через запятую в ожидаемом
        }


        [Theory]
        [InlineData("4", "f(2)+g(2)", "f(x) = x/2", "g(x) = x+1")]
        [InlineData("9", "f(2)+g(2)+t(5)", "f(x) = x/2", "g(x) = x+1", "t(x) = x")]
        public void IncludedFunctionsInContext(string expected, string expression, params string[] context)
        {
            var ctx = new Context();
            foreach (var c in context)
                ctx.DefineFunction(c);
            Assert.Equal(Calculator.Parser.Parse(expression, ctx).Eval(ctx).ToString(), expected); // программно числа возвращаются с запятой, а анализируются с точкой, в ассертах надо 
            // тип дабл указывать через запятую в ожидаемом
        }

        [Theory]
        [InlineData("1", "38*a+b-c+1", "a=0", "b=1", "c=1")]
        public void IncludedConstsInContext(string expected, string expression, params string[] context)
        {
            var ctx = new Context();
            foreach (var c in context)
                ctx.DefineConstant(c);
            Assert.Equal(Calculator.Parser.Parse(expression, ctx).Eval(ctx).ToString(), expected); // программно числа возвращаются с запятой, а анализируются с точкой, в ассертах надо 
            // тип дабл указывать через запятую в ожидаемом
        }
        [Theory]
        [InlineData("10", "a+f(5)", new string[]{"a=5"}, new string[] { "f(x)=x"})]
        [InlineData("10", "a+f(1)", new string[] { "a=5" }, new string[] { "f(x)=a*x" })]
        public void CombinedCaseFuncsAndConsts(string expected, string expression, string[] consts, string[] funcs)
        {
            var ctx = new Context();
            foreach (var f in funcs)
                ctx.DefineFunction(f);
            foreach (var c in consts)
                ctx.DefineConstant(c);
            Assert.Equal(Calculator.Parser.Parse(expression, ctx).Eval(ctx).ToString(), expected); // программно числа возвращаются с запятой, а анализируются с точкой, в ассертах надо 
            // тип дабл указывать через запятую в ожидаемом
        }

        [Theory]
        [InlineData(30, "10+20")]
        [InlineData(-10, "10-20")]
        [InlineData(90, "10+20-40+100")]
        public void AddSubtractTest(double number, string expression)
        {
            Assert.Equal(number.ToString(), Calculator.Parser.Parse(expression).Eval(null).ToString());
        }



        [Theory]
        [InlineData(200, "10*20")]
        [InlineData(0.5, "10/20")]
        [InlineData(25, "10/20*50")]
        public void MultiplyDivideTest(double number, string expression)
        {
            Assert.Equal(number.ToString(), Calculator.Parser.Parse(expression).Eval(null).ToString());
        }

        [Theory]
        [InlineData(610, "10 + 20 * 30")]
        [InlineData(-900, "-(10 + 20) * 30")]
        [InlineData(-4500, " - ((10 + 20) * 5) * 30")]
        public void OrderOfOperation(double number, string expression)
        {
            Assert.Equal(number.ToString(), Calculator.Parser.Parse(expression).Eval(null).ToString());
        }


        [Fact]
        public void Variables()
        {
            Context ctx = new Context();

            Assert.Equal(Calculator.Parser.Parse("2.0 * PI * E").Eval(ctx), 2 * Math.PI * Math.E);
        }

    }
}
