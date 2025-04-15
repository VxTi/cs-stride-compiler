namespace Stride.Compiler.Ast.Nodes;

public class FunctionInvocationNode(Symbol name, List<Expression> arguments) : AstNode
{
    public override string ToString()
    {
        return $"Invoke ${name} ({string.Join(", ", arguments)})";
    }

    public bool IsPure()
    {
        // TODO: Implemente
        return false;
    }
}