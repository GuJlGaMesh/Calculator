namespace Calculator
{
    public interface IContext
    {
        double CallFunction(string name, Node[] arguments);
        double ResolveVariable(string name);
    }
}
