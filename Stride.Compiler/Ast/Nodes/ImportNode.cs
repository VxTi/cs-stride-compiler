namespace Stride.Compiler.Ast.Nodes;

public class ImportNode(List<ImportNode.Import> imports) : AstNode
{
    public override void Validate()
    {
        // TODO: Validate imports
    }

    public override string ToString()
    {
        return @"ImportNode: " + string.Join(", ", imports);
    }

    public record Import(List<string> NamespaceSequence)
    {
        public override string ToString()
        {
            return string.Join(".", NamespaceSequence);
        }
    }
}