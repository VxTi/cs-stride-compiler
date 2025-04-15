using Stride.Compiler.Ast.Nodes;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public class FunctionInvocationNodeFactory : AbstractTreeNodeFactory
{
    public override void Synthesize(TokenSet set, AstNode rootNode, ContextMetadata? metadata)
    {
        var functionSymbol = Symbol.FromTokenSet(set);
        var arguments = GetInvocationArgumentsFromSet(set);
        var invocation = new FunctionInvocationNode(functionSymbol, arguments);

        rootNode.Children.Add(invocation);
    }

    private static List<Expression> GetInvocationArgumentsFromSet(TokenSet set)
    {
        List<Expression> arguments = new();


        return arguments;
    }

    public override LexicalScope GetLexicalScope()
    {
        return LexicalScope.Block;
    }

    public override bool CanConsumeToken(Token nextToken, TokenSet set)
    {
        return nextToken.Type == TokenType.Identifier
               && set.PeekEqual(TokenType.LParenthesis, 1);
    }
}