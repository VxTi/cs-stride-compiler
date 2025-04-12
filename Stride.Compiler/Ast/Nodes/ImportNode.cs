namespace Stride.Compiler.Ast.Nodes;

public class ImportNode(List<ImportNode.Import> imports) : AstNode
{
    public override string ToString()
    {
        return @"ImportNode: " + string.Join(", ", imports);
    }

    public record Import(Symbol Name)
    {
        public override string ToString()
        {
            return string.Join(".", Name);
        }
    }
}