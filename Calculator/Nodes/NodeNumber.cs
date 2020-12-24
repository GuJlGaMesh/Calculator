namespace Calculator
{

    public class NodeNumber : Node
    {
        public NodeNumber(double number)
        {
            _number = number;
        }

        double _number;           

        public override double Eval(IContext ctx)
        {
            return _number;
        }
    }
}
