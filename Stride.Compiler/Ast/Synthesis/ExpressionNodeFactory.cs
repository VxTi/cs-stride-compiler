using Stride.Common.Logging;
using Stride.Compiler.Exceptions;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public class ExpressionNodeFactory : AbstractTreeNodeFactory
{
    public override void Synthesize(TokenSet set, AstNode rootNode, ContextMetadata? metadata)
    {
        var exprLen = 0;
        while (!set.PeekEqual(TokenType.Semicolon))
        {
            var next = set.Peek();
            if (next == null || !IsTokenAllowed(next.Type))
                throw new IllegalStateException($"Unexpected token in expression: {next?.Type}");
            
            set.Next();
            exprLen++;
        }

        set.Consume(TokenType.Semicolon);
        Logger.Info($"Expression {exprLen + 1}");
    }

    private static bool IsTokenAllowed(TokenType type)
    {
        return type switch
        {
            TokenType.Identifier or TokenType.Comma or TokenType.Ampersand
                => true,
            _ => false
        };
    }

    public override LexicalScope GetLexicalScope()
    {
        return LexicalScope.Global;
    }

    public override bool CanConsumeToken(Token nextToken, TokenSet set)
    {
        return nextToken.Type is TokenType.NumberFloat
            or TokenType.NumberInteger or TokenType.StringLiteral
            or TokenType.CharLiteral or TokenType.Identifier;
    }
}