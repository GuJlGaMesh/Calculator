using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{

    public class Context : IContext
    {
        private Dictionary<string, NodeEmbeddedFunctionFactory> _funcsAndConsts = new Dictionary<string, NodeEmbeddedFunctionFactory>(StringComparer.OrdinalIgnoreCase)
        {
            { "ln", new NodeEmbeddedFunctionFactory((Func<double, double>)Math.Log) },
            { "lb", new NodeEmbeddedFunctionFactory((Func<double, double>)Math.Log10) },
            { "log", new NodeEmbeddedFunctionFactory((Func<double, double, double>)Math.Log) },
            { "sin", new NodeEmbeddedFunctionFactory((Func<double, double>)Math.Sin) },
            {"cos", new NodeEmbeddedFunctionFactory((Func<double, double>)Math.Cos) },
            {"asin", new NodeEmbeddedFunctionFactory((Func<double, double>)Math.Asin ) },
            {"acos", new NodeEmbeddedFunctionFactory((Func<double, double>)Math.Acos )},
            {"tan", new NodeEmbeddedFunctionFactory((Func<double, double>)Math.Tan )},
            {"cot", new NodeEmbeddedFunctionFactory((Func<double, double>) (a => 1.0 / Math.Tan(a)) )},
            {"atan", new NodeEmbeddedFunctionFactory((Func<double, double>)Math.Atan )},
            {"acot", new NodeEmbeddedFunctionFactory((Func<double, double>)(a => Math.PI / 2 - Math.Atan(a)) )},
            {"sqrt", new NodeEmbeddedFunctionFactory((Func<double, double>)Math.Sqrt )},
            {"abs", new NodeEmbeddedFunctionFactory((Func<double, double>)Math.Abs )},
            {"fact", new NodeEmbeddedFunctionFactory((Func<long, long>)Fact )},
            {"pow", new NodeEmbeddedFunctionFactory((Func<double, double, double>)Math.Pow) },
            {"E", new NodeEmbeddedFunctionFactory((Func<double>)( ()=> Math.E))  },
            {"PI", new NodeEmbeddedFunctionFactory((Func<double>)( ()=> Math.PI))  }
        };
        private List<string> _userFuncsAndConsts = new List<string>();
        private static long Fact(long arg)
        {
            if (arg == 0)
            {
                return 1;
            }
            else
            {
                return arg * Fact(arg - 1);
            }
        }

        public double CallFunction(string name, Node[] arguments) => GetDelegate(name, arguments).Eval(this);

        public Node GetDelegate(string name, Node[] arguments)
        {
            if (_funcsAndConsts.TryGetValue(name, out var factory))
            {
                return factory.Create(arguments);
            }
            throw new NotImplementedException("Функция не определена.");
        }

        public Node GetDelegate(string name)
        {
            if (_funcsAndConsts.TryGetValue(name, out var factory))
            {
                return factory.Create();
            }
            throw new NotImplementedException("константа не определена.");
        }
        public double ResolveVariable(string name) => GetDelegate(name).Eval(this);

        public void DefineConstant (string consts)
        {
            _userFuncsAndConsts.Add(consts);
        }
        public void DefineFunction(string function)
        {
            _userFuncsAndConsts.Add(function + ";");
        }
        public List<string> GetUserContext() => _userFuncsAndConsts;
    }
}
