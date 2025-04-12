namespace Stride.Compiler.Ast.Nodes;

public class PackageNode(Symbol name) : AstNode
{
    public override string ToString()
    {
        return $"PackageNode ({string.Join(".", name)})";
    }
}