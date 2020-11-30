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
        
        // Static helper to parse a string
        public static Node Parse(string str) => Parse(new Tokenizer(new StringReader(Prepare(str))));

        // variables with equal pattern \b(\w)+(\s)*[=](\s)*(\d)+[.]?(\d)*\b
        // just vars (^([a-zA-Z]+)([a-zA-Z0-9])*)
        // only digits + . ([0-9]*)([^a-zA-Z]*)([.]?)([0-9]*)$
        // function pattern 
        /*
        public static string Prepare(string str)
        {
            var tokenizer = new Tokenizer(new StringReader(str));
            //tokenizer.NextToken();
            while (str.Contains(";"))
            {
                if (tokenizer.Token == Token.Identifier)
                {
                    var name = tokenizer.Identifier;
                    tokenizer.NextToken();
                    if (tokenizer.Token == Token.Equal)
                    {
                        tokenizer.NextToken();
                        if (tokenizer.Token == Token.Number)
                        {
                            var number = tokenizer.Number.ToString();
                            str = str.Replace(name, number);
                            str = str.Replace(str.Substring(0, str.IndexOf(";") + 1), "");  
                        }
                        // else just trim that part of str

                    }
                    else 
                        str = str.Replace(str.Substring(0, str.IndexOf(";") + 1), "");

                   
                }
                tokenizer.NextToken();
            }
            return str;
        }
    */
        
        private static string PrepareConsts(string str)
        {
            var consts = new Regex(@"([a-zA-Z]+)\w*(\s)*[=](\s)*([0-9][^a-zA-Z])+([.])?([0-9])*");
            var onlyVars = new Regex(@"(^([a-zA-Z]+)([a-zA-Z0-9])*)");
            var digit = new Regex(@"([0-9]+)([^a-zA-Z]*)([.]?)([0-9]*)$");
            var matchList = consts.Matches(str);
            foreach (var match in matchList)
            {
                var mtch = match.ToString();
                string varName = onlyVars.Match(mtch).ToString();
                string dgt = digit.Match(mtch).ToString();
                while (str.Contains(varName))
                    str = str.Replace(varName, varName.ToUpper());
                varName = varName.ToUpper();
                str = str.Replace(mtch.ToUpper(), "");
                str = str.Replace(varName, dgt);
                //str = str.Replace(consts.Match(mtch.ToUpper()).ToString(), " ");
            }
            return str;
        }
        //([a-zA-Z]+([a-zA-Z0-9])*)+(\s)*[(]((\s*)([a-zA-Z]+([a-zA-Z0-9])*)+(\s)*)+[,]?(([a-zA-Z]+([a-zA-Z0-9])*)+(\s)*[,?])*(\s)*(([a-zA-Z]+([a-zA-Z0-9])*)+(\s)*)*[)]\s+[=].*[;]$
        private static string PrepareFunctions(string str)
        {
            string funcPattern = @"([a-zA-Z]+([a-zA-Z0-9])*)+(\s)*[(]((\s*)([a-zA-Z]+([a-zA-Z0-9])*)+(\s)*)+[,]?(([a-zA-Z]+([a-zA-Z0-9])*)+(\s)*[,?])*(\s)*(([a-zA-Z]+([a-zA-Z0-9])*)+(\s)*)*[)]\s*[=]\s*(([a-z+\-\/\*0-9\(\)\s\,])+)(\s*)[;]";
            var func = new Regex(funcPattern);
            var funcCallPattern = @"([a-zA-Z]+([a-zA-Z0-9])*)+(\s)*[(]((\s*)([\w]+([\w])*)+(\s)*)+[,]?(([\w]+([\w])*)+(\s)*[,?])*(\s)*(([\w]+([\w])*)+(\s)*)*[)]";
            var funcCall = new Regex(funcCallPattern);
            var funcList= func.Matches(str);
            var funcCalls = funcCall.Matches(str);

            foreach (var f in funcList)
            {
                var funcCallOfCurrentF = funcCall.Match(f.ToString()).Value;
                var funcParams = GetParametersOfFuncCall(funcCallOfCurrentF);
                var nameF = GetFuncName(f.ToString());
                var funcToReplace = f.ToString().Substring(f.ToString().IndexOf('=') + 1).Trim(';',' '); // получаем выражение после = в строке вида "f(x)=x;"
                foreach (var fc in funcCalls)
                { 
                    if (nameF.Equals(GetFuncName(fc.ToString()))&& !funcCallOfCurrentF.Equals(fc.ToString()))
                    {
                        string toReplace = funcToReplace.Clone().ToString();
                        var relationMap = ParametersRelation(funcParams, GetParametersOfFuncCall(fc.ToString()));
                        for (int i = 0; i < funcParams.Length; i++)
                        {
                            toReplace = toReplace.Replace(funcParams[i], relationMap[funcParams[i]]);
                        }
                        str = str.Replace(fc.ToString(), '(' + toReplace + ')');
                    }
                }
                str = str.Replace(f.ToString(), "");
               
            }

            return str;
        }
        private static Dictionary<string,string> ParametersRelation(string[] key, string[] value)
        {
            var dict = new Dictionary<string, string>();
            if (key.Length == value.Length)
            {
                for (int i = 0; i < key.Length; i++)
                {
                    dict.Add(key[i], value[i]);
                }
            }
            else
                throw new Exception("Разные размеры массивов");
            return dict;
        }
        private static string[] GetParametersOfFuncCall(string str)
        {
            str = str.Remove(0, str.IndexOf('(') + 1);
            str = str.Remove(str.IndexOf(')'), 1);
            return str.Split(',');
        }
        private static string GetFuncName(string str) => str.Substring(0, str.IndexOf("("));

        private static string Prepare(string str)
        {
            str = PrepareConsts(str);
            str = PrepareFunctions(str);
            return str;
            
        }
        // Static helper to parse from a tokenizer
        public static Node Parse(Tokenizer tokenizer)
        {
            var parser = new Parser(tokenizer);
            return parser.ParseExpression();
        }

        #endregion
    }
}
