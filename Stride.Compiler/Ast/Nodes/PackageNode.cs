namespace Stride.Compiler.Ast.Nodes;

public class PackageNode(string[] packageNesting) : AstNode
{
    public override string ToString()
    {
        return $"PackageNode ({string.Join(".", packageNesting)})";
    }
}