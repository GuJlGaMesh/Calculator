using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Calculator
{
    public class NodeEmbeddedFunction : Node
    {
        private readonly Delegate _function;
        private readonly Node[] _arguments;

        public NodeEmbeddedFunction(Delegate function, Node[] arguments)
        {
            // todo: Можно добавить проверку, что функция соотвтетствует числу аргуменов и возвращвемому значению
            if (function.Method.ReturnType != typeof(double))
            {
                throw new ArgumentException("Функция имеет неверную сигнатуру");
            }
            _function = function;
            _arguments = arguments;
        }

        public NodeEmbeddedFunction(Delegate function)
        {
            if (function.Method.ReturnType != typeof(double))
            {
                throw new ArgumentException("Функция имеет неверную сигнатуру");
            }
            _function = function;
            _arguments = null;
        }

        public override double Eval(IContext ctx)
        {
            if (_arguments != null)
            {
                var values = _arguments.Select(arg => arg.Eval(ctx))
                    .Cast<object>() // это нужно чтобы передать массив в DynamicInvoke как массимв объектов, иначе массив doublе воспринимается как подин из params
                    .ToArray();
                return (double)_function.DynamicInvoke(values);
            }
            else
                return (double)_function.DynamicInvoke();
        }
    }
}
