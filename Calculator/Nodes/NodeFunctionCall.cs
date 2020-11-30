namespace Calculator
{
    public class NodeFunctionCall : Node
    {
        public NodeFunctionCall(string functionName, Node[] arguments)
        {
            _functionName = functionName;
            _arguments = arguments;
        }

        private readonly string _functionName;
        private readonly Node[] _arguments;

        public override double Eval(IContext ctx) => ctx.CallFunction(_functionName, _arguments);
    }
}
