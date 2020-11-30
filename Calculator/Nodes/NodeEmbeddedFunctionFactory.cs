using System;
using System.Collections.Generic;
using System.Linq;
using Calculator;
namespace Calculator
{

    public class NodeEmbeddedFunctionFactory
    {
        private readonly Delegate _function;

        public NodeEmbeddedFunctionFactory(Delegate function)
        {
            _function = function;
        }

        public Node Create(Node[] arguments)
        {
            return new NodeEmbeddedFunction(_function, arguments);
        }

        public Node Create()
        {
            return new NodeEmbeddedFunction(_function);
        }
    }



}
