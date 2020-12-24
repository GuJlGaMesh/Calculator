using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Calculator.Utils
{
    static class Preparator
    {
        private static string PrepareConsts(string str)
        {
            var consts = new Regex(@"([a-zA-Z]+)\w*(\s)*[=](\s)*\d+(\.\d+)?");
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
            var funcList = func.Matches(str);
            var funcCalls = funcCall.Matches(str);

            foreach (var f in funcList)
            {
                var funcCallOfCurrentF = funcCall.Match(f.ToString()).Value;
                var funcParams = GetParametersOfFuncCall(funcCallOfCurrentF);
                var nameF = GetFuncName(f.ToString());
                var funcToReplace = f.ToString().Substring(f.ToString().IndexOf('=') + 1).Trim(';', ' '); // получаем выражение после = в строке вида "f(x)=x;"
                foreach (var fc in funcCalls)
                {
                    if (nameF.Equals(GetFuncName(fc.ToString())) && !funcCallOfCurrentF.Equals(fc.ToString()))
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
        private static Dictionary<string, string> ParametersRelation(string[] key, string[] value)
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

        public static string Prepare(string str)
        {
            str = PrepareConsts(str);
            str = PrepareFunctions(str);
            return str;
        }
        public static string Prepare(string str, Context ctx)
        {
            var toAdd = ctx.GetUserContext();
            string context = "";
            foreach (var u in toAdd)
            {
                context += u + " ";
            }
            str = context + str;
            str = PrepareConsts(str);
            str = PrepareFunctions(str);
            return str;
        }
    }
}
