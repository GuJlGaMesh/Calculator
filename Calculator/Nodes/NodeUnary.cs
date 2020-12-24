using System;

namespace Calculator
{
    // Унарный
    public class NodeUnary : Node
    {
        public NodeUnary(Node rhs, Func<double, double> op)
        {
            _rhs = rhs;
            _op = op;
        }

        Node _rhs;                          
        Func<double, double> _op;            

        public override double Eval(IContext ctx)
        {

            var rhsVal = _rhs.Eval(ctx);
            var result = _op(rhsVal);
            return result;
        }
    }
}
