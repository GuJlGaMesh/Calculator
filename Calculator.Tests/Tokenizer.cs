using System;
using System.IO;
using Xunit;


namespace Calculator.Tests
{
    public class Tokenizer_Test
    {
        [Fact]
        public void Tokens()
        {
            string testString = "10 + 20 - 30.123";
            Tokenizer t = new Tokenizer(new StringReader(testString));

            // "10"
            Assert.Equal(Token.Number, t.Token );
            Assert.Equal( 10, t.Number);
            t.NextToken();

            // "+"
            Assert.Equal(Token.Add, t.Token);
            t.NextToken();

            // "20"
            Assert.Equal(Token.Number, t.Token);
            Assert.Equal(20, t.Number);
            t.NextToken();

            // "-"
            Assert.Equal(Token.Subtract, t.Token);
            t.NextToken();

            // "30.123"
            Assert.Equal(Token.Number, t.Token);
            Assert.Equal(30.123, t.Number);
            t.NextToken();
        }
    }
}
