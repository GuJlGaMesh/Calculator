using System;
using System.IO;
using Xunit;
namespace Calculator.Tests
{
    public class UnitTest1
    {

        [Theory]
        [InlineData("a=5 c =3 a+c", "8")]
        //[InlineData("c = 3.85 (PI*E+c)*c", "(PI*E+3.85)*3.85")]
        [InlineData("f(x) = x/2; g(x) = x+1; f(2)+g(2)", "4")]
        public void MultiVarTest(string str,string expected)
        {
            var ctx = new EmbeddedContext();
            Assert.Equal(Parser.Parse(str).Eval(ctx).ToString(), expected); // программно числа возвращаютс€ с зап€той, а анализируютс€ с точкой, в ассертах надо 
            // тип дабл указывать через зап€тую в ожидаемом
        }
        [Fact]
        public void Contains()
        {
            var str = "— = 3.85 (PI*E+—)*c";
            Assert.True(str.Contains("c") == true);
        }
        [Fact]
        public void TokenizerTest()
        {
            string testString = "10 + 20 - 30.123";
            Tokenizer t = new Tokenizer(new StringReader(testString));

            // "10"
            Assert.Equal(t.Token, Token.Number);
            Assert.Equal(t.Number, 10);
            t.NextToken();

            // "+"
            Assert.Equal(t.Token, Token.Add);
            t.NextToken();

            // "20"
            Assert.Equal(t.Token, Token.Number);
            Assert.Equal(t.Number, 20);
            t.NextToken();

            // "-"
            Assert.Equal(t.Token, Token.Subtract);
            t.NextToken();

            // "30.123"
            Assert.Equal(t.Token, Token.Number);
            Assert.Equal(t.Number, 30.123);
            t.NextToken();
        }

        [Fact]
        public void AddSubtractTest()
        {
            Assert.Equal(Parser.Parse("10 + 20").Eval(null), 30);

            Assert.Equal(Parser.Parse("10 - 20").Eval(null), -10);

            Assert.Equal(Parser.Parse("10 + 20 - 40 + 100").Eval(null), 90);
        }

        [Fact]
        public void UnaryTest()
        {

            Assert.Equal(Parser.Parse("-10").Eval(null), -10);

            Assert.Equal(Parser.Parse("+10").Eval(null), 10);

            Assert.Equal(Parser.Parse("--10").Eval(null), 10);

            Assert.Equal(Parser.Parse("--++-+-10").Eval(null), 10);

            Assert.Equal(Parser.Parse("10 + -20 - +30").Eval(null), -40);
        }

        [Fact]
        public void MultiplyDivideTest()
        {
            Assert.Equal(Parser.Parse("10 * 20").Eval(null), 200);

            Assert.Equal(Parser.Parse("10 / 20").Eval(null), 0.5);

            Assert.Equal(Parser.Parse("10 * 20 / 50").Eval(null), 4);
        }

        [Fact]
        public void OrderOfOperation()
        {

            Assert.Equal(Parser.Parse("10 + 20 * 30").Eval(null), 610);

            Assert.Equal(Parser.Parse("(10 + 20) * 30").Eval(null), 900);

            Assert.Equal(Parser.Parse("-(10 + 20) * 30").Eval(null), -900);

            Assert.Equal(Parser.Parse("-((10 + 20) * 5) * 30").Eval(null), -4500);
        }

        [Fact]
        public void StandartContext()
        {
            EmbeddedContext ctx = new EmbeddedContext();
            Assert.Equal(8.33, Parser.Parse("kek = 5 f(x, y) = x + y * kek; sin(PI * cos(f(PI, 0)) / 2) + f(1,0)").Eval(ctx));
        }

        [Fact]
        public void Variables()
        {
            EmbeddedContext ctx = new EmbeddedContext();

            Assert.Equal(Parser.Parse("2.0 * PI * E").Eval(ctx), 2 * Math.PI * Math.E);
        }

    }
}
