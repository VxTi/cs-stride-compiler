namespace Stride.Compiler.Ast;

public abstract class AstNode
{
    public readonly AstNode? Parent = null;
    public readonly List<AstNode> Children;
    
    public bool IsRootNode => Parent == null;

    protected AstNode(AstNode? parent, List<AstNode> children)
    {
        Parent = parent;
        Children = children;
    }

    protected AstNode(AstNode? parent)
    {
        Parent = parent;
        Children = [];
    }

    protected AstNode()
    {
        Children = [];
    }
    
    public class RootNode : AstNode
    {
    }
    
    public virtual bool IsReducible() { return false; }

    public virtual AstNode? Reduce()
    {
        return null;
    }

    public override string ToString()
    {
        return string.Join("\n", Children.Select(child => child.ToString()));
    }
}