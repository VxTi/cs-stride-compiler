using Stride.Compiler.Ast.Nodes;
using Stride.Compiler.Exceptions;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public class PackageNodeFactory : AbstractTreeNodeFactory
{
    
    public override AstNode? Synthesize(TokenSet tokens)
    {
        tokens.Consume(TokenType.KeywordPackage);

        var nextToken = tokens.Next();
        
        if (nextToken is not { Type: TokenType.Identifier })
            throw new IllegalTokenSequenceException("Expected package name, but got " + nextToken?.Type);
        
        List<string> packageNesting = [nextToken.Value];
        
        do
        {
            if (tokens.PeekEqual(TokenType.Dot))
            {
                tokens.Next();
                nextToken = tokens.Next();
                
                if (nextToken is not { Type: TokenType.Identifier })
                    throw new IllegalTokenSequenceException("Expected package name");
                
                packageNesting.Add(nextToken.Value);
            }
            else
            {
                break;
            }
        } while (!tokens.PeekEqual(TokenType.Semicolon));

        tokens.Consume(TokenType.Semicolon); // consume semicolon
        
        return new PackageNode(packageNesting.ToArray());
    }

    public override LexicalScope GetLexicalScope()
    {
        return LexicalScope.TopLevel;
    }

    public override bool CanConsumeToken(Token nextToken)
    {
        return nextToken.Type == TokenType.KeywordPackage;
    }
}