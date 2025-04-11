namespace Stride.Compiler.Ast.Nodes;

public class PackageNode(string[] packageNesting) : AstNode
{
    public override void Validate()
    {
        // TODO: Check whether packageNesting is valid
    }

    public override string ToString()
    {
        return $"PackageNode ({string.Join(".", packageNesting)})";
    }
}