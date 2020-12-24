using Calculator.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Calculator
{
    public class Parser
    {
        public Parser(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        Tokenizer _tokenizer;

        public Node ParseExpression()
        {
            var expr = ParseAddSubtract();

            if (_tokenizer.Token != Token.EOF)
                throw new SyntaxException("Unexpected characters at end of expression");
            return expr;
        }

        Node ParseAddSubtract()
        {
            var lhs = ParseMultiplyDivide();

            while (true)
            {
                Func<double, double, double> op = null;
                if (_tokenizer.Token == Token.Add)
                {
                    op = (a, b) => a + b;
                }
                else if (_tokenizer.Token == Token.Subtract)
                {
                    op = (a, b) => a - b;
                }

                if (op == null)
                    return lhs;             // no

                _tokenizer.NextToken();

                var rhs = ParseMultiplyDivide();
                lhs = new NodeBinary(lhs, rhs, op);
            }
        }

        Node ParseMultiplyDivide()
        {
            var lhs = ParseUnary();

            while (true)
            {
                Func<double, double, double> op = null;
                if (_tokenizer.Token == Token.Multiply)
                {
                    op = (a, b) => a * b;
                }
                else if (_tokenizer.Token == Token.Divide)
                {
                    op = (a, b) => a / b;
                }


                if (op == null)
                    return lhs;             

                _tokenizer.NextToken();


                var rhs = ParseUnary();
                lhs = new NodeBinary(lhs, rhs, op);
            }
        }


        // Унарный оператор
        Node ParseUnary()
        {
            while (true)
            {

                if (_tokenizer.Token == Token.Add)
                {
                    _tokenizer.NextToken();
                    continue;
                }

                if (_tokenizer.Token == Token.Subtract)
                {

                    _tokenizer.NextToken();

                    // Рекурсия для поддержки положительных и отрицательных
                    var rhs = ParseUnary();

                    return new NodeUnary(rhs, (a) => -a);
                }

                // Нет знаков, тогда парс листа
                return ParseLeaf();
            }
        }
        //Парс листа
        Node ParseLeaf()
        {
            // 
            if (_tokenizer.Token == Token.Number)
            {
                var node = new NodeNumber(_tokenizer.Number);
                _tokenizer.NextToken();
                return node;
            }

            // 
            if (_tokenizer.Token == Token.OpenParens)
            {
                _tokenizer.NextToken();
                var node = ParseAddSubtract();

                if (_tokenizer.Token != Token.CloseParens)
                    throw new SyntaxException("Missing close parenthesis");
                _tokenizer.NextToken();

                return node;
            }

            if (_tokenizer.Token == Token.Identifier)
            {

                var name = _tokenizer.Identifier;
                _tokenizer.NextToken();
                // Определение функция или рпеменная по наличиб скобки
                if (_tokenizer.Token != Token.OpenParens)
                {
                    return new NodeVariable(name);
                }
                else
                {

                    _tokenizer.NextToken();

                    var arguments = new List<Node>();
                    while (true)
                    {
                        arguments.Add(ParseAddSubtract());
                        // Другой аргумент?
                        if (_tokenizer.Token == Token.Comma)
                        {
                            _tokenizer.NextToken();
                            continue;
                        }

                        break;
                    }

                    if (_tokenizer.Token != Token.CloseParens)
                        throw new SyntaxException("Missing close parenthesis");
                    _tokenizer.NextToken();

                    return new NodeFunctionCall(name, arguments.ToArray());
                }
            }
            throw new SyntaxException($"Unexpect token: {_tokenizer.Token}");
        }


        #region Convenience Helpers
        public static Node Parse(string str, Context cxt) => Parse(new Tokenizer(new StringReader(Preparator.Prepare(str,cxt))));
        public static Node Parse(string str) => Parse(new Tokenizer(new StringReader(Preparator.Prepare(str))));
        
        
        // Static helper to parse from a tokenizer
        public static Node Parse(Tokenizer tokenizer)
        {
            var parser = new Parser(tokenizer);
            return parser.ParseExpression();
        }

        #endregion
    }
}
