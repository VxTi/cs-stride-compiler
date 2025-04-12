using Stride.Compiler.Exceptions;

namespace Stride.Compiler.Ast.Nodes;

public class Expression
{
    public class AdditionNode(AstNode left, AstNode right) : AstNode, IReducible<AtomicValueNode>
    {
        public AtomicValueNode Reduce()
        {
            if (!IsReducible())
                throw new IllegalStateException($"Unable to reduce addition node due to illegal left hand node type: {left}");
            
            var leftNode = (AtomicValueNode)left;
            var rightNode = (AtomicValueNode)right;

            if (leftNode.IsString())
                return new((string)leftNode.Value + (string)rightNode.Value);

            if (leftNode.IsFloat() || rightNode.IsFloat())
                return new((float)leftNode.Value + (float)rightNode.Value);

            return new((long)leftNode.Value + (long)rightNode.Value);
        }

        public bool IsReducible()
        {
            return left is AtomicValueNode && right is AtomicValueNode;
        }
    }

    public class SubtractionNode(AstNode left, AstNode right) : AstNode, IReducible<AtomicValueNode>
    {
        public AtomicValueNode Reduce()
        {
            if (!IsReducible())
                throw new IllegalStateException($"Unable to reduce subtraction node due to illegal left hand node type: {left}");
            
            var leftNode = (AtomicValueNode)left;
            var rightNode = (AtomicValueNode)right;

            if (leftNode.IsFloat() || rightNode.IsFloat())
                return new((float)leftNode.Value - (float)rightNode.Value);

            return new((long)leftNode.Value - (long)rightNode.Value);
        }

        public bool IsReducible()
        {
            return left is AtomicValueNode && right is AtomicValueNode;
        }
    }

    public class MultiplicationNode(AstNode left, AstNode right) : AstNode, IReducible<AtomicValueNode>
    {
        public AtomicValueNode Reduce()
        {
            throw new NotImplementedException();
        }

        public bool IsReducible()
        {
            return left is AtomicValueNode && right is AtomicValueNode;
        }
    }

    public class DivisionNode(AstNode left, AstNode right) : AstNode, IReducible<AtomicValueNode>
    {
        public AtomicValueNode Reduce()
        {
            throw new NotImplementedException();
        }

        public bool IsReducible()
        {
            return left is AtomicValueNode && right is AtomicValueNode;
        }
    }
    
}  