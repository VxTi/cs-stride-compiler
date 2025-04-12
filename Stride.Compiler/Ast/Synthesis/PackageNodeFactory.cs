using Stride.Common.Logging;
using Stride.Compiler.Ast.Nodes;
using Stride.Compiler.Exceptions;
using Stride.Compiler.Tokenization;

namespace Stride.Compiler.Ast.Synthesis;

public class PackageNodeFactory : AbstractTreeNodeFactory
{
    
    public override void Synthesize(TokenSet set, AstNode rootNode)
    {
        set.Consume(TokenType.KeywordPackage);

        var nextToken = set.Next();
        
        if (nextToken is not { Type: TokenType.Identifier })
            throw new IllegalTokenSequenceException("Expected package name, but got " + nextToken?.Type);
        
        List<string> packageNesting = [nextToken.Value];
        
        do
        {
            if (set.PeekEqual(TokenType.Dot))
            {
                set.Next();
                nextToken = set.Next();
                
                if (nextToken is not { Type: TokenType.Identifier })
                    throw new IllegalTokenSequenceException("Expected package name");
                
                packageNesting.Add(nextToken.Value);
            }
            else
            {
                break;
            }
        } while (!set.PeekEqual(TokenType.Semicolon) && set.Remaining() > 0);

        set.Consume(TokenType.Semicolon); // consume semicolon
        
        Logger.Info($"Package {string.Join(".", packageNesting)} has been synced");
        
        rootNode.Children.Add(new PackageNode(packageNesting.ToArray()));
    }

    public override PermittedLexicalScope GetLexicalScope()
    {
        return PermittedLexicalScope.Global;
    }

    public override bool CanConsumeToken(Token nextToken)
    {
        return nextToken.Type == TokenType.KeywordPackage;
    }
}