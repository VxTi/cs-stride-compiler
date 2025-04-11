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
    
    public abstract void Validate();
    
    public class RootNode : AstNode
    {
        public override void Validate()
        {
            foreach (var child in Children)
            {
                child.Validate();
            }
        }
    }
}