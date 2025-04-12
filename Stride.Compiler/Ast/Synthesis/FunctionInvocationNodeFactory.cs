using Stride.Compiler.Ast.Nodes;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public class FunctionInvocationNodeFactory : AbstractTreeNodeFactory
{
    public override void Synthesize(TokenSet set, AstNode rootNode)
    {
        var functionName = set.RequiresNext(TokenType.Identifier).Value;
        var invocation = new FunctionInvocationNode(
            functionName,
            );
        
        rootNode.Children.Add(invocation);
    }

    public override PermittedLexicalScope GetLexicalScope()
    {
        return PermittedLexicalScope.Block;
    }

    public override bool CanConsumeToken(Token nextToken, TokenSet set)
    {
        return nextToken.Type == TokenType.Identifier
            && set.PeekEqual(TokenType.LParenthesis, 1);
    }
}